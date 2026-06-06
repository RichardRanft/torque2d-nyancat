//-----------------------------------------------------------------------------
// Copyright (c) 2013 Roostertail Games
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to
// deal in the Software without restriction, including without limitation the
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
// sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// IN THE SOFTWARE.
//-----------------------------------------------------------------------------

function initializeGameStateEventManager()
{
    if (!isObject(GameStateEventManager))
    {
        $GameStateEventManager = new EventManager(GameStateEventManager)
        { 
            queue = "GameStateEventManager"; 
        };
        
        // Module related signals
        GameStateEventManager.registerEvent("_UpdateRequest");
        GameStateEventManager.registerEvent("_Created");
        GameStateEventManager.registerEvent("_Initialized");
        GameStateEventManager.registerEvent("_StateLoaded");
        GameStateEventManager.registerEvent("_StateSaved");
        GameStateEventManager.registerEvent("_StartSave");
        GameStateEventManager.registerEvent("_StartLoad");
        GameStateEventManager.registerEvent("_StartNewGame");
        GameStateEventManager.registerEvent("_NewGameStarted");
        GameStateEventManager.registerEvent("_RequestLoadScreen");
        GameStateEventManager.registerEvent("_RequestLoadScreenClear");
        GameStateEventManager.registerEvent("_QuitRequest");
    }
    
    if (!isObject(GameStateListener))
    {
        $GameStateListener = new ScriptMsgListener(GameStateListener) 
        { 
            class = "GameStateEventListener"; 
        };
        
        // Module related subscriptions
        GameStateEventManager.subscribe(GameStateListener, "_UpdateRequest", "onUpdateRequest");
        GameStateEventManager.subscribe(GameStateListener, "_Created", "onCreated");
        GameStateEventManager.subscribe(GameStateListener, "_Initialized", "onInitialized");
        GameStateEventManager.subscribe(GameStateListener, "_StateLoaded", "onStateLoaded");
        GameStateEventManager.subscribe(GameStateListener, "_StateSaved", "onStateSaved");
        GameStateEventManager.subscribe(GameStateListener, "_StartSave", "onStartSave");
        GameStateEventManager.subscribe(GameStateListener, "_StartLoad", "onStartLoad");
        GameStateEventManager.subscribe(GameStateListener, "_StartNewGame", "onStartNewGame");
        GameStateEventManager.subscribe(GameStateListener, "_NewGameStarted", "onNewGameStarted");
        GameStateEventManager.subscribe(GameStateListener, "_RequestLoadScreen", "onRequestLoadScreen");
        GameStateEventManager.subscribe(GameStateListener, "_RequestLoadScreenClear", "onRequestLoadScreenClear");
        GameStateEventManager.subscribe(GameStateListener, "_QuitRequest", "onQuitRequest");
    }
}

// Cleanup the GameStateEventManager
function destroyGameStateEventManager()
{
    if (isObject(GameStateEventManager) && isObject(GameStateListener))
    {
        // Remove all the subscriptions
        GameStateEventManager.remove(GameStateListener, "_UpdateRequest");
        GameStateEventManager.remove(GameStateListener, "_Created");
        GameStateEventManager.remove(GameStateListener, "_Initialized");
        GameStateEventManager.remove(GameStateListener, "_StateLoaded");
        GameStateEventManager.remove(GameStateListener, "_StateSaved");
        GameStateEventManager.remove(GameStateListener, "_StartSave");
        GameStateEventManager.remove(GameStateListener, "_StartLoad");
        GameStateEventManager.remove(GameStateListener, "_NewGameStarted");
        GameStateEventManager.remove(GameStateListener, "_StartNewGame");
        GameStateEventManager.remove(GameStateListener, "_RequestLoadScreen");
        GameStateEventManager.remove(GameStateListener, "_RequestLoadScreenClear");
        GameStateEventManager.remove(GameStateListener, "_QuitRequest");

        // Delete the actual objects
        GameStateEventManager.delete();
        GameStateListener.delete();
        
        // Clear the global variables, just in case
        $GameStateEventManager = "";
        $GameStateListener = "";
    }
}

function GameStateEventListener::onUpdateRequest(%this, %msgData)
{
    // handle GameState update requests
    echo(" @@@ GameStateEventListener::onUpdateRequest(" @ %this @ ", " @ %msgData @")");
}

function GameStateEventListener::onCreated(%this, %msgData)
{
    // GameState::Create() completed
    %msgData.initialize();
    echo(" @@@ GameStateEventListener::onCreated(" @ %this @ ", " @ %msgData @")");
}

function GameStateEventListener::onInitialized(%this, %msgData)
{
	echo(" @@@ GameState " @ %msgData @ " Initialized");
}

function GameStateEventListener::onStateLoaded(%this, %msgData)
{
    // the game state has been loaded
    echo(" @@@ GameStateEventListener::onStateLoaded(" @ %this @ ", " @ %msgData @")");
}

function GameStateEventListener::onStateSaved(%this, %msgData)
{
    // the game state has been saved
    echo(" @@@ GameStateEventListener::onStateSaved(" @ %this @ ", " @ %msgData @")");
}

function GameStateEventListener::onStartNewGame(%this, %msgData)
{
    // create new game state from defaults
    echo(" @@@ GameStateEventListener::onStartNewGame(" @ %this @ ", " @ %msgData @")");
}

function GameStateEventListener::onNewGameStarted(%this, %msgData)
{
    // new game state has been created
    echo(" @@@ GameStateEventListener::onNewGameStarted(" @ %this @ ", " @ %msgData @")");
}

function GameStateEventListener::onStartSave(%this, %msgData)
{
    // prepare to save the game
    echo(" @@@ GameStateEventListener::onStartSave(" @ %this @ ", " @ %msgData @")");
}

function GameStateEventListener::onStartLoad(%this, %msgData)
{
    // prepare to load the game
    echo(" @@@ GameStateEventListener::onStartLoad(" @ %this @ ", " @ %msgData @")");
}

function GameStateEventListener::onRequestLoadScreen(%this, %msgData)
{
    echo(" @@@ GameStateEventListener::onRequestLoadScreen(" @ %this @ ", " @ %msgData @")");
}

function GameStateEventListener::onRequestLoadScreenClear(%this, %msgData)
{
    echo(" @@@ GameStateEventListener::onRequestLoadScreenClear(" @ %this @ ", " @ %msgData @")");
}

function GameStateEventListener::onQuitRequest(%this, %msgData)
{
    // prepare to load the game
    echo(" @@@ GameStateEventListener::onQuitRequest(" @ %this @ ", " @ %msgData @")");
}