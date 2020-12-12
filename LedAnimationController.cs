﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using PhoenixLeds.DTO;

namespace PhoenixLeds
{
    public static class LedAnimationController
    {
        private static List<LedAnimation> LedAnimations { get; set; } = new List<LedAnimation>();
        private static List<Panel> PlayingAnimations { get; set; } = new List<Panel>();

        /// <summary>
        /// Load animations from .anim files in Animations directory
        /// </summary>
        public static async Task LoadAnimationsAsync() {
            var animationsPath = Path.Combine(".", "Animations");

            if (!Directory.Exists(animationsPath)) {
                Console.WriteLine($"Error: Animations directory does not exist at '{animationsPath}'");
                return;
            }

            var animFiles = Directory.GetFiles(animationsPath, "*.anim");

            if (!animFiles.Any()) {
                Console.WriteLine("Error: Could not find any .anim files in the Animations directory.");
                return;
            }

            Console.WriteLine($"Found {animFiles.Length} animation files.");

            var ledAnimationDtoTasks = animFiles.Select(LoadAnimationAsync);
            var ledAnimationDtos = (await Task.WhenAll(ledAnimationDtoTasks))
                .Where(x => x != null && !string.IsNullOrWhiteSpace(x.AnimationTexturePath))
                .GroupBy(x => x.Name).Select(x => x.First())
                .ToList();

            var ledAnimations = ledAnimationDtos.Select(x => new LedAnimation(
                x.Name,
                x.AnimationType,
                x.RotateToFaceCenterPanel,
                x.AnimationFrameWidth,
                x.AnimationFrameHeight,
                TextureLoader.LoadAnimationFramesFromTexture(
                    x.AnimationFrameWidth, x.AnimationFrameHeight,
                    x.AnimationTexturePath, x.RotateToFaceCenterPanel
                ),
                x.FramesPerSecond
            )).ToList();

            Console.WriteLine($"Loaded {ledAnimations.Count} valid animations.");

            LedAnimations = ledAnimations;
        }

        private static async Task<LedAnimationDto?> LoadAnimationAsync(string path) {
            var json = await File.ReadAllTextAsync(path, Encoding.UTF8);

            if (string.IsNullOrWhiteSpace(json)) {
                Console.WriteLine($"Error: .anim file '{path}' is empty! Skipping.");
                return null;
            }

            var options = new JsonSerializerOptions {
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
            };

            LedAnimationDto ledAnimationDto;

            try {
                ledAnimationDto = JsonSerializer.Deserialize<LedAnimationDto>(json, options);
            }
            catch (JsonException e) {
                Console.WriteLine($"Error: Could not parse .anim file '{path}': {e.Message}");
                return null;
            }

            if (string.IsNullOrWhiteSpace(ledAnimationDto.Name)) {
                Console.WriteLine($"Error: Led animation '{path}' must have a name!");
                return null;
            }

            if (string.IsNullOrWhiteSpace(ledAnimationDto.AnimationTexturePath)) {
                Console.WriteLine($"Error: Led animation '{path}' is missing animation texture path!");
                return null;
            }

            Console.WriteLine($"Path: {path}");

            // If the texture path is a relative path, we must adjust it to be relative of .anim file and not the executing assembly
            if (ledAnimationDto.AnimationTexturePath.StartsWith('.')) {
                var animFileDirectory = Path.GetDirectoryName(path);
                var relativeTexturePath = animFileDirectory + Path.DirectorySeparatorChar;

                if (ledAnimationDto.AnimationTexturePath.StartsWith(".."))
                    relativeTexturePath += ledAnimationDto.AnimationTexturePath;
                else
                    relativeTexturePath += ledAnimationDto.AnimationTexturePath.Substring(2);

                ledAnimationDto.AnimationTexturePath = relativeTexturePath;
            }

            Console.WriteLine($"Loaded animation '{ledAnimationDto.Name}'");
            return ledAnimationDto;
        }

        /// <summary>
        /// Play an animation on 1-4 panels
        /// </summary>
        /// <param name="animationName">The name of the animation to play</param>
        /// <param name="panels">Which panels to play the animation on, can contain only unique values</param>
        public static async Task PlayLedAnimationAsync(string animationName, HashSet<Panel> panels) {
            if (!panels.Any()) {
                Console.WriteLine($"No panels specified for the animation!");
                return;
            }

            var animation = LedAnimations.SingleOrDefault(x => x.Name == animationName);

            if (animation == null) {
                Console.WriteLine($"Animation with name '{animationName}' not found!");
                return;
            }
        }

        /// <summary>
        /// Play an animation on a panel
        /// </summary>
        /// <param name="animationName">The name of the animation to play</param>
        /// <param name="panel">The panel to play the animation on</param>
        public static async Task PlayLedAnimationAsync(string animationName, Panel panel) {
            await PlayLedAnimationAsync(animationName, new HashSet<Panel> { panel });
        }
    }
}
