using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CPI311.GameEngine
{
    public static class InputManager
    {
        private static KeyboardState PreviousKeyboardState { get; set; }
        private static KeyboardState CurrentKeyboardState { get; set; }
        private static MouseState PreviousMouseState { get; set; }
        private static MouseState CurrentMouseState { get; set; }

        // ********** Lab 8 ************** //
        public static bool IsMousePressed(int mouseButton)
        {
            switch(mouseButton)
            {
                case 0: return PreviousMouseState.LeftButton == ButtonState.Released && CurrentMouseState.LeftButton == ButtonState.Pressed;
                case 1: return PreviousMouseState.RightButton == ButtonState.Released && CurrentMouseState.RightButton == ButtonState.Pressed;
                case 2: return PreviousMouseState.MiddleButton == ButtonState.Released && CurrentMouseState.MiddleButton == ButtonState.Pressed;
                default: return false;
            }
        }

        public static bool IsMouseReleased(int mouseButton)
        {
            switch (mouseButton)
            {
                case 0: return PreviousMouseState.LeftButton == ButtonState.Pressed && CurrentMouseState.LeftButton == ButtonState.Released;
                case 1: return PreviousMouseState.RightButton == ButtonState.Pressed && CurrentMouseState.RightButton == ButtonState.Released;
                case 2: return PreviousMouseState.MiddleButton == ButtonState.Pressed && CurrentMouseState.MiddleButton == ButtonState.Released;
                default: return false;
            }
        }

        public static Vector2 GetMousePosition()
        {
            return new Vector2(CurrentMouseState.X, CurrentMouseState.Y);
        }
        // ******************************* //

        public static void Initialize()
        {
            PreviousKeyboardState = CurrentKeyboardState = Keyboard.GetState();
            PreviousMouseState = CurrentMouseState = Mouse.GetState();
        }
        public static void Update()
        {
            PreviousKeyboardState = CurrentKeyboardState;
            CurrentKeyboardState = Keyboard.GetState();
            PreviousMouseState = CurrentMouseState;
            CurrentMouseState = Mouse.GetState();
        }

        public static bool IsKeyDown(Keys key) { return CurrentKeyboardState.IsKeyDown(key); }
        public static bool IsKeyUp(Keys key) { return CurrentKeyboardState.IsKeyUp(key); }
        public static bool IsKeyPressed(Keys key) { return CurrentKeyboardState.IsKeyDown(key) && PreviousKeyboardState.IsKeyUp(key); }
        public static bool IsKeyReleased(Keys key) { return CurrentKeyboardState.IsKeyUp(key) && PreviousKeyboardState.IsKeyDown(key); }


    }
}