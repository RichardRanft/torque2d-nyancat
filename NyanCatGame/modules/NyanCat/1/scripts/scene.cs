//-----------------------------------------------------------------------------
// Copyright (c) 2013 GarageGames, LLC
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

function createNyanCatWindow()
{
    // Sanity!
    if ( !isObject(NyanCatWindow) )
    {
        // Create the scene window.
        new SceneWindow(NyanCatWindow);

        // Set profile.        
        NyanCatWindow.Profile = NyanCatWindowProfile;
        
        // Push the window.
        Canvas.setContent( NyanCatWindow );                     
    }

    // Set camera to a canonical state.
    %allBits = "0 1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25 26 27 28 29 30 31";
    NyanCatWindow.stopCameraMove();
    NyanCatWindow.dismount();
    NyanCatWindow.setViewLimitOff();
    NyanCatWindow.setRenderGroups( %allBits );
    NyanCatWindow.setRenderLayers( %allBits );
    NyanCatWindow.setObjectInputEventGroupFilter( %allBits );
    NyanCatWindow.setObjectInputEventLayerFilter( %allBits );
    NyanCatWindow.setLockMouse( true );
    NyanCatWindow.setCameraPosition( 0, 0 );
    NyanCatWindow.setCameraSize( 100, 75 );
    NyanCatWindow.setCameraZoom( 1 );
    NyanCatWindow.setCameraAngle( 0 );
    NyanCatWindow.setUseWindowInputEvents(true);
}

//-----------------------------------------------------------------------------

function destroyNyanCatWindow()
{
    // Finish if no window available.
    if ( !isObject(NyanCatWindow) )
        return;
    
    // Delete the window.
    NyanCatWindow.delete();
}

//-----------------------------------------------------------------------------

function createNyanCatScene()
{
    // Destroy the scene if it already exists.
    if ( isObject(NyanCatScene) )
        destroyNyanCatScene();
    
    // Create the scene.
    new Scene(NyanCatScene);
            
    // Sanity!
    if ( !isObject(NyanCatWindow) )
    {
        error( "NyanCat: Created scene but no window available." );
        return;
    }
    NyanCatScene.setGravity(0, 0);
        
    // Set window to scene.
    setSceneToWindow();    
}

//-----------------------------------------------------------------------------

function destroyNyanCatScene()
{
    // Finish if no scene available.
    if ( !isObject(NyanCatScene) )
        return;

    // Delete the scene.
    NyanCatScene.delete();
}

//-----------------------------------------------------------------------------

function setSceneToWindow()
{
    // Sanity!
    if ( !isObject(NyanCatScene) )
    {
        error( "Cannot set NyanCat Scene to Window as the Scene is invalid." );
        return;
    }
    
     // Set scene to window.
    NyanCatWindow.setScene( NyanCatScene );

    // Set camera to a canonical state.
    %allBits = "0 1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25 26 27 28 29 30 31";
    NyanCatWindow.stopCameraMove();
    NyanCatWindow.dismount();
    NyanCatWindow.setViewLimitOff();
    NyanCatWindow.setRenderGroups( %allBits );
    NyanCatWindow.setRenderLayers( %allBits );
    NyanCatWindow.setObjectInputEventGroupFilter( %allBits );
    NyanCatWindow.setObjectInputEventLayerFilter( %allBits );
    NyanCatWindow.setLockMouse( true );
    NyanCatWindow.setCameraPosition( 0, 0 );
    NyanCatWindow.setCameraSize( 100, 75 );
    NyanCatWindow.setCameraZoom( 1 );
    NyanCatWindow.setCameraAngle( 0 );
    NyanCatWindow.setUseWindowInputEvents(true);
    
    // reset the NyanCat manipulation modes.
    NyanCat.resetManipulationModes();       
}

function NyanCatWindow::onTouchDown(%this, %touchID, %worldPosition)
{
    %this.picked = NyanCatScene.pickPoint( %worldPosition );
    if( isObject(%this.picked) )
    {
        %count = getWordCount(%this.picked);
        for( %i = 0; %i < %count; %i++ )
        {
            %obj = getWord(%this.picked, %i);
            %obj.callOnBehaviors("onTouchDown", %touchID, %worldPosition);
        }
    }
}

function NyanCatScene::onEndCollision(%this, %objBuffer, %infoBuffer)
{
    echo(" --- NyanCatScene::onEndCollision()");
}
//-----------------------------------------------------------------------------

function setCustomScene( %scene )
{
    // Sanity!
    if ( !isObject(%scene) )
    {
        error( "Cannot set NyanCat to use an invalid Scene." );
        return;
    }
   
    // Destroy the existing scene.  
    destroyNyanCatScene();

    // The NyanCat needs the scene to be named this.
    %scene.setName( "NyanCatScene" );    
    
    // Set the scene to the window.
    setSceneToWindow();
}
