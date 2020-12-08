namespace PhoenixLeds
{
    public enum GlobalLightState
    {
        None, // All lights are off
        Idle, // Running idle animation
        SelectSong, // Running select song animation
        GamePlayChart, // Animate the played chart with note colors
        GamePlayJudgements, // Animate lights based on note judgements and their colors
        ScoreResults, // Result screen animation
        InputTesting // Testing input, animate input values
    }

    public enum AnimationType
    {
        PlayOnce,
        Loop
    }

    public enum Panel
    {
        Left,
        Down,
        Up,
        Right
    }
}
