using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class SkillInputs
{
    private static readonly SkillInput[] R3_RightToUp = { SkillInput.R3_Right, SkillInput.R3_Up };
    private static readonly SkillInput[] R3_RightToDown = { SkillInput.R3_Right, SkillInput.R3_Down };

    private static readonly SkillInput[] R3_UpToRight = { SkillInput.R3_Up, SkillInput.R3_Right };
    private static readonly SkillInput[] R3_UpToLeft = { SkillInput.R3_Right, SkillInput.R3_Left };

    private static readonly SkillInput[] R3_DownToRight = { SkillInput.R3_Down, SkillInput.R3_Right };
    private static readonly SkillInput[] R3_DownToLeft = { SkillInput.R3_Down, SkillInput.R3_Left };

    private static readonly SkillInput[] R3_LeftToUp = { SkillInput.R3_Left, SkillInput.R3_Up };
    private static readonly SkillInput[] R3_LeftToDown = { SkillInput.R3_Left, SkillInput.R3_Down };
    private static readonly SkillInput[] R3_LeftToDownToLeft = { SkillInput.R3_Left, SkillInput.R3_Down, SkillInput.R3_Left };
    private static readonly SkillInput[] R3_LeftToUpToLeft = { SkillInput.R3_Left, SkillInput.R3_Up, SkillInput.R3_Left };


    public static SkillInput? GetTabInput(string buttonName)
    {
        return (buttonName) switch
        {
            ("buttonSouth") => SkillInput.Button_X,
            ("buttonEast") => SkillInput.Button_Circle,
            ("buttonWest") => SkillInput.Button_Square,
            ("buttonNorth") => SkillInput.Button_Triangle,

            ("leftShoulder") => SkillInput.L1,
            ("rightShoulder") => SkillInput.R1,
            _ => null
        };
    }

    public static SkillInput? GetHoldInput(string buttonName)
    {
        return (buttonName) switch
        {
            ("buttonSouth") => SkillInput.Hold_Button_X,
            ("buttonEast") => SkillInput.Hold_Button_Circle,
            ("buttonWest") => SkillInput.Hold_Button_Square,
            ("buttonNorth") => SkillInput.Hold_Button_Triangle,

            ("leftShoulder") => SkillInput.L1_Hold,
            ("rightShoulder") => SkillInput.R1_Hold,
            _ => null
        };
    }

    public static SkillInput? GetStickInput(string buttonName, bool isLeft = true, bool isHeld = false)
    {
        if (isLeft)
        {
            if (!isHeld)
                return (buttonName) switch
                {
                    ("up") => SkillInput.L3_Up,
                    ("right") => SkillInput.L3_Right,
                    ("down") => SkillInput.L3_Down,
                    ("left") => SkillInput.L3_Left,
                    _ => null
                };
            else
            {
                return (buttonName) switch
                {
                    ("up") => SkillInput.Hold_L3_Up,
                    ("right") => SkillInput.Hold_L3_Right,
                    ("down") => SkillInput.Hold_L3_Down,
                    ("left") => SkillInput.Hold_L3_Left,
                    _ => null
                };
            }
        }
        else
        {
            if (!isHeld)
            {
                return (buttonName) switch
                {
                    ("up") => SkillInput.R3_Up,
                    ("right") => SkillInput.R3_Right,
                    ("down") => SkillInput.R3_Down,
                    ("left") => SkillInput.R3_Left,
                    _ => null
                };
            }
            else
            {
                return (buttonName) switch
                {
                    ("up") => SkillInput.Hold_R3_Up,
                    ("right") => SkillInput.Hold_R3_Right,
                    ("down") => SkillInput.Hold_R3_Down,
                    ("left") => SkillInput.Hold_R3_Left,
                    _ => null
                };

            }
        }
    }

    public static SkillInput? GetStickRotationInput(string buttonName, List<SkillInput> input)
    {
        if (buttonName.Contains("rightStick"))
        {
            if (input.SequenceEqual(R3_RightToUp))
                return SkillInput.R3_RightToUp;

            if (input.SequenceEqual(R3_RightToDown))
                return SkillInput.R3_RightToDown;

            if (input.SequenceEqual(R3_UpToRight))
                return SkillInput.R3_UpToRight;

            if (input.SequenceEqual(R3_UpToLeft))
                return SkillInput.R3_UpToLeft;

            if (input.SequenceEqual(R3_DownToRight))
                return SkillInput.R3_DownToRight;

            if (input.SequenceEqual(R3_DownToLeft))
                return SkillInput.R3_DownToLeft;

            if (input.SequenceEqual(R3_LeftToUp))
                return SkillInput.R3_LeftToUp;

            if (input.SequenceEqual(R3_LeftToDown))
                return SkillInput.R3_LeftToDown;

            if (input.SequenceEqual(R3_LeftToDownToLeft))
                return SkillInput.R3_LeftToDownToLeft;

            if (input.SequenceEqual(R3_LeftToUpToLeft))
                return SkillInput.R3_LeftToUpToLeft;
        }


        return null;
    }

    public static SkillInput? GetFlickDiagonalInput(Vector2 stickPosition, bool isLeft = true)
    {
        float degrees = Mathf.Atan2(stickPosition.y, stickPosition.x) * Mathf.Rad2Deg;
        if (degrees < 0f)
            degrees += 360f;

        if (degrees >= 20f && degrees < 70f)
        {
            return isLeft ? SkillInput.L3_UpRight : SkillInput.R3_UpRight;
        }
        else if (degrees >= 110f && degrees < 160f)
        {
            return isLeft ? SkillInput.L3_UpLeft : SkillInput.R3_UpLeft;
        }
        else if (degrees >= 200f && degrees < 250f)
        {
            return isLeft ? SkillInput.L3_DownLeft : SkillInput.R3_DownLeft;
        }
        else if (degrees >= 290f && degrees <= 340f)
        {
            return isLeft ? SkillInput.L3_DownRight : SkillInput.R3_DownRight;
        }
        return SkillInput.None;
    }
}
