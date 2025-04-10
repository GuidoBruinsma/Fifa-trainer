using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class SkillInputs
{
    private static readonly SkillInput[] R3_RightToUp = { SkillInput.R3_Right, SkillInput.R3_Up };
    private static readonly SkillInput[] R3_RightToDown = { SkillInput.R3_Right, SkillInput.R3_Down };

    private static readonly SkillInput[] R3_UpToRight = { SkillInput.R3_Up, SkillInput.R3_Right };
    private static readonly SkillInput[] R3_UpToLeft = { SkillInput.R3_Up, SkillInput.R3_Left };

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
            ("leftStickPress") => SkillInput.L3,
            ("rightStickPress") => SkillInput.R3,
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

            ("leftTrigger") => SkillInput.L2_Hold,
            ("rightTrigger") => SkillInput.R2_Hold,
            _ => null
        };
    }

    public static SkillInput? GetStickRotationInput(string buttonName, List<SkillInput> input)
    {
        bool isRightStick = buttonName.Contains("rightStick");
        bool isLeftStick = buttonName.Contains("leftStick");

        if (!isRightStick && !isLeftStick)
            return null;

        if (input.SequenceEqual(R3_RightToUp))
            return isRightStick ? SkillInput.R3_RightToUp : SkillInput.L3_RightToUp;

        if (input.SequenceEqual(R3_RightToDown))
            return isRightStick ? SkillInput.R3_RightToDown : SkillInput.L3_RightToDown;

        if (input.SequenceEqual(R3_UpToRight))
            return isRightStick ? SkillInput.R3_UpToRight : SkillInput.L3_UpToRight;

        if (input.SequenceEqual(R3_UpToLeft))
            return isRightStick ? SkillInput.R3_UpToLeft : SkillInput.L3_UpToLeft;

        if (input.SequenceEqual(R3_DownToRight))
            return isRightStick ? SkillInput.R3_DownToRight : SkillInput.L3_DownToRight;

        if (input.SequenceEqual(R3_DownToLeft))
            return isRightStick ? SkillInput.R3_DownToLeft : SkillInput.L3_DownToLeft;

        if (input.SequenceEqual(R3_LeftToUp))
            return isRightStick ? SkillInput.R3_LeftToUp : SkillInput.L3_LeftToUp;

        if (input.SequenceEqual(R3_LeftToDown))
            return isRightStick ? SkillInput.R3_LeftToDown : SkillInput.L3_LeftToDown;

        if (input.SequenceEqual(R3_LeftToDownToLeft))
            return isRightStick ? SkillInput.R3_LeftToDownToLeft : SkillInput.L3_LeftToDownToLeft;

        if (input.SequenceEqual(R3_LeftToUpToLeft))
            return isRightStick ? SkillInput.R3_LeftToUpToLeft : SkillInput.L3_LeftToUpToLeft;

        return null;
    }

    public static SkillInput? GetFlickDiagonalInput(Vector2 stickPosition, bool isLeft = true, bool isHeld = false)
    {
        float degrees = Mathf.Atan2(stickPosition.y, stickPosition.x) * Mathf.Rad2Deg;
        if (degrees < 0f)
            degrees += 360f;
        /*
         * 70 to 110 == UP
         * 160 to 200 == left
         * 250 to 290 down
         * 340 to 20 right
         * 
         * if(< 20 && > 340)
         */

        //Debug.Log(degrees);
        if (!isHeld)
        {
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

            if (degrees <= 20f || degrees > 340f)
            {
                return isLeft ? SkillInput.L3_Right : SkillInput.R3_Right; // right
            }
            else if (degrees >= 70f && degrees < 110f)
            {
                return isLeft ? SkillInput.L3_Up : SkillInput.R3_Up; // up
            }
            else if (degrees >= 160f && degrees < 200f)
            {
                return isLeft ? SkillInput.L3_Left : SkillInput.R3_Left; // left
            }
            else if (degrees >= 250f && degrees < 290f)
            {
                return isLeft ? SkillInput.L3_Down : SkillInput.R3_Down; // down
            }
        }
        else
        {
            if (degrees >= 20f && degrees < 70f)
            {
                return isLeft ? SkillInput.Hold_L3_UpRight : SkillInput.Hold_R3_UpRight;
            }
            else if (degrees >= 110f && degrees < 160f)
            {
                return isLeft ? SkillInput.Hold_L3_UpLeft : SkillInput.Hold_R3_UpLeft;
            }
            else if (degrees >= 200f && degrees < 250f)
            {
                return isLeft ? SkillInput.Hold_L3_DownLeft : SkillInput.Hold_R3_DownLeft;
            }
            else if (degrees >= 290f && degrees <= 340f)
            {
                return isLeft ? SkillInput.Hold_L3_DownRight : SkillInput.Hold_R3_DownRight;
            }

            if (degrees <= 20f || degrees > 340f)
            {
                return isLeft ? SkillInput.Hold_L3_Right : SkillInput.Hold_R3_Right;
                //right
            }
            else if (degrees >= 70f && degrees < 110f)
            {
                return isLeft ? SkillInput.Hold_L3_Up : SkillInput.Hold_R3_Up;
                //Up
            }
            else if (degrees >= 160f && degrees < 200f)
            {
                return isLeft ? SkillInput.Hold_L3_Left : SkillInput.Hold_R3_Left;
                //left
            }
            else if (degrees >= 250f && degrees < 290f)
            {
                return isLeft ? SkillInput.Hold_L3_Down : SkillInput.Hold_R3_Down;
                //down
            }

        }
        return SkillInput.Flick_None;
    }

    public static bool IsL3Input(SkillInput? input)
    {
        return input == SkillInput.L3_Left || input == SkillInput.L3_Right ||
               input == SkillInput.L3_Up || input == SkillInput.L3_Down ||
               input == SkillInput.L3_UpRight || input == SkillInput.L3_UpLeft ||
               input == SkillInput.L3_DownRight || input == SkillInput.L3_DownLeft;
    }

    public static bool IsR3Input(SkillInput? input)
    {
        return input == SkillInput.R3_Left || input == SkillInput.R3_Right ||
               input == SkillInput.R3_Up || input == SkillInput.R3_Down ||
               input == SkillInput.R3_UpRight || input == SkillInput.R3_UpLeft ||
               input == SkillInput.R3_DownRight || input == SkillInput.R3_DownLeft;
    }

    public static bool IsL3HoldInput(SkillInput? input)
    {
        return input == SkillInput.Hold_L3_Left || input == SkillInput.Hold_L3_Right ||
               input == SkillInput.Hold_L3_Up || input == SkillInput.Hold_L3_Down ||
               input == SkillInput.Hold_L3_UpRight || input == SkillInput.Hold_L3_UpLeft ||
               input == SkillInput.Hold_L3_DownRight || input == SkillInput.Hold_L3_DownLeft;
    }

    public static bool IsR3HoldInput(SkillInput? input)
    {
        return input == SkillInput.Hold_R3_Left || input == SkillInput.Hold_R3_Right ||
               input == SkillInput.Hold_R3_Up || input == SkillInput.Hold_R3_Down ||
               input == SkillInput.Hold_R3_UpRight || input == SkillInput.Hold_R3_UpLeft ||
               input == SkillInput.Hold_R3_DownRight || input == SkillInput.Hold_R3_DownLeft;
    }
}
