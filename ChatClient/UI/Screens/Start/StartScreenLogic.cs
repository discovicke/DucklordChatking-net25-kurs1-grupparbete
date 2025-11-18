﻿using ChatClient.Core;
 using ChatClient.Core.Application;
 using ChatClient.Core.Infrastructure;
 using ChatClient.Core.Input;
 using ChatClient.Data;
using ChatClient.Data.Services;
using ChatClient.UI.Components;
using ChatClient.UI.Components.Base;
using ChatClient.UI.Components.Specialized;
using ChatClient.UI.Components.Text;
using ChatClient.UI.Screens.Common;
using Raylib_cs;

namespace ChatClient.UI.Screens.Start
{
    /// <summary>
    /// Responsible for: handling user input and navigation logic for the start/login screen.
    /// Manages login button clicks, field updates, tab navigation, and transitions to register/options/chat screens.
    /// </summary>
    public class StartScreenLogic(
        TextField userField,
        TextField passwordField,
        Button loginButton,
        Button registerButton) 
        : IScreenLogic
    {
        // DEV MODE: Set to false before production release
        private const bool DEV_MODE_ENABLED = true;
        
        private readonly UserAuth userAuth = new UserAuth(ServerConfig.CreateHttpClient());
        public readonly FeedbackBox FeedbackBox = new();

        private readonly TabLogics tabs = new();
        private bool tabsInitialized;
        public void HandleInput()
        {
            // Register fields once in desired tab order (username -> password)
            if (!tabsInitialized)
            {
                tabs.Register(userField);
                tabs.Register(passwordField);
                tabsInitialized = true;
            }

            FeedbackBox.Update();

            // DEV MODE: Ctrl+Shift+D for instant dev login
            if (DEV_MODE_ENABLED &&
                Raylib.IsKeyDown(KeyboardKey.LeftControl) &&
                Raylib.IsKeyDown(KeyboardKey.LeftShift) &&
                Raylib.IsKeyPressed(KeyboardKey.D))
            {
                DevLogin();
                return;
            }
            tabs.Update();

            userField.Update();
            passwordField.Update();

            if (MouseInput.IsLeftClick(loginButton.Rect) || Raylib.IsKeyPressed(KeyboardKey.Enter))
            {
                Login();
            }

            if (MouseInput.IsLeftClick(registerButton.Rect))
            {
                NavigateToRegister();
            }
        }

        private void Login()
        {
            string username = userField.Text.Trim();
            string password = passwordField.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                FeedbackBox.Show("Please enter quackername and password!", false);
                Log.Info("[StartScreenLogic] Login failed - Username or password empty");
                return;
            }

            Log.Info($"[StartScreenLogic] Login attempt - Username: '{username}'");

            bool success = userAuth.Login(username, password);

            if (success)
            {
                Log.Success($"[StartScreenLogic] Login successful for user '{username}'");
                AppState.LoggedInUsername = username;
                FeedbackBox.Show($"Welcome duck, {username}!", true);

                Task.Delay(1000).ContinueWith(_ =>
                {
                    AppState.CurrentScreen = Screen.Chat;
                    ClearFields();
                });
            }
            else
            {
                Log.Error($"[StartScreenLogic] Login failed for user '{username}' - Invalid credentials");
                FeedbackBox.Show("DUCK! Login failed, check your credentials.", false);
            }
        }

        private void DevLogin()
        {
            Log.Info("[StartScreenLogic] DEV MODE: Quack login activated (Ctrl+Shift+D)");
    
            // Actual login with server validation
            bool success = userAuth.Login("Ducklord", "chatking");
    
            if (success)
            {
                Log.Success("[StartScreenLogic] DEV MODE: Login successful for Ducklord");
                AppState.LoggedInUsername = "Ducklord";
                FeedbackBox.Show("DEV MODE: Welcome back, Ducklord!", true);
        
                Task.Delay(1000).ContinueWith(_ =>
                {
                    AppState.CurrentScreen = Screen.Chat;
                    ClearFields();
                });
            }
            else
            {
                Log.Error("[StartScreenLogic] DEV MODE: Login failed for Ducklord - Invalid credentials");
                FeedbackBox.Show("DEV MODE: Login failed!", false);
            }
        }

        private void NavigateToRegister()
        {
            Log.Info("[StartScreenLogic] Navigating to register screen");
            AppState.CurrentScreen = Screen.Register;
            ClearFields();
        }

        /*
        private void NavigateToOptions()
        {
            Log.Info("[StartScreenLogic] Navigating to options screen");
            AppState.CurrentScreen = Screen.Options;
            ClearFields();
        }
        */

        private void ClearFields()
        {
            Log.Info("[StartScreenLogic] Clearing all fields");
            userField.Clear();
            passwordField.Clear();
        }
    }
}
