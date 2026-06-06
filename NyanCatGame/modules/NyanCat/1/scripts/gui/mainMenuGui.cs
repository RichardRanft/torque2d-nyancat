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

function toggleMainMenu(%make)
{
    // Finish if being released.
    if ( !%make )
        return;
        
    // Finish if the console is awake.
    if ( ConsoleDialog.isAwake() )
        return;       
        
    // Is the main menu awake?
    if ( MainMenu.isAwake() )
    {
        // Yes, so deactivate it.
        if ( $enableDirectInput )
            activateKeyboard();
        Canvas.popDialog(MainMenu);
        MainOverlay.setVisible(1);
        if (NyanCat.menuMusicHandle)
        	alxStop(NyanCat.menuMusicHandle);
        return;
    }
    
    // Activate it.
    if ( $enableDirectInput )
        deactivateKeyboard();

    MainOverlay.setVisible(0);
    Canvas.pushDialog(MainMenu);
    %pageWidth = mainMenuBackground.Extent.x;
    %titleWidth = mainMenuTitle.Extent.x;
    %posX = (%pageWidth / 2) - (%titleWidth / 2);
    %pos = %posX SPC "32";
    mainMenuTitle.Position = %pos;
    if( NyanCat.menuMusicHandle !$= "" )
        alxStop( NyanCat.menuMusicHandle );
    if( isObject(NyanCatScene) )
        NyanCatScene.setScenePause(true);
    NyanCat.menuMusicHandle = alxPlay("NyanCatAssets:NyanCat");
}

function showMainMenu()
{
    if ( MainMenu.isAwake() )
        return;
    // Activate it.
    if ( $enableDirectInput )
        deactivateKeyboard();

    MainOverlay.setVisible(0);
    Canvas.pushDialog(MainMenu);
    %pageWidth = mainMenuBackground.Extent.x;
    %titleWidth = mainMenuTitle.Extent.x;
    %posX = (%pageWidth / 2) - (%titleWidth / 2);
    %pos = %posX SPC "32";
    mainMenuTitle.Position = %pos;
    if( NyanCat.menuMusicHandle !$= "" )
        alxStop( NyanCat.menuMusicHandle );
    if( isObject(NyanCatScene) )
        NyanCatScene.setScenePause(true);
    NyanCat.menuMusicHandle = alxPlay("NyanCatAssets:NyanCat");
}

function menuNewButton::onClick(%this)
{
	GameStateEventManager.postEvent("_StartNewGame", "");
}

function menuExitButton::onClick(%this)
{
    GameStateEventManager.postEvent("_QuitRequest", 500);
}

function MainMenu::onDialogPush(%this)
{
	if (NyanCat.gameState)
		menuSaveButton.setActive(true);
}