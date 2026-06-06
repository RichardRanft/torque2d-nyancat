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

// This module is intended to handle managing a global GameState.
function GameState::create( %this )
{    
    // Load the preferences.
    %this.loadPreferences();
    
    // Load GameState scripts.
    exec( "./scripts/gameStateEventManager.cs" );
    exec( "./scripts/gameState.cs" );

    // initialize event manager
    initializeGameStateEventManager();
	GameStateEventManager.subscribe(%this, "_StateLoaded", "onStateLoaded");
	GameStateEventManager.subscribe(%this, "_StateSaved", "onStateSaved");
	GameStateEventManager.subscribe(%this, "_StartSave", "onStartSave");
	GameStateEventManager.subscribe(%this, "_StartLoad", "onStartLoad");
	GameStateEventManager.subscribe(%this, "_StartNewGame", "onStartNewGame");
	GameStateEventManager.subscribe(%this, "_NewGameStarted", "onNewGameStarted");
	GameStateEventManager.subscribe(%this, "_RequestLoadScreen", "onRequestLoadScreen");
	GameStateEventManager.subscribe(%this, "_RequestLoadScreenClear", "onRequestLoadScreenClear");
	GameStateEventManager.subscribe(%this, "_QuitRequest", "onQuitRequest");
	GameStateEventManager.postEvent("_Created", %this);
}

//-----------------------------------------------------------------------------

function GameState::destroy( %this )
{
	GameStateEventManager.remove(%this, "_StateLoaded");
	GameStateEventManager.remove(%this, "_StateSaved");
	GameStateEventManager.remove(%this, "_StartSave");
	GameStateEventManager.remove(%this, "_StartLoad");
	GameStateEventManager.remove(%this, "_NewGameStarted");
	GameStateEventManager.remove(%this, "_StartNewGame");
	GameStateEventManager.remove(%this, "_QuitRequest");
    destroyGameStateEventManager();
    %this.savePreferences();
}

//-----------------------------------------------------------------------------

function GameState::loadPreferences( %this )
{
    // Load the default preferences.
    exec( "./scripts/gameStatePreferences.cs" );
    
    // Load the last session preferences if available.
    if ( isFile("preferences.cs") )
        exec( "preferences.cs" );   
}

//-----------------------------------------------------------------------------

function GameState::savePreferences( %this )
{
    // Export only the GameState preferences.
    export("$pref::GameState::*", "preferences.cs", false );
}
