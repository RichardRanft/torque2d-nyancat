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

// NyanCat manipulation modes are:
// - Off
// - Pan
// - Pull
NyanCat.ManipulationMode = "off";

// Reset the NyanCat pull object.
NyanCat.ManipulationPullMaxForce = 1000;

// Reset the touch events.
NyanCat.TouchController = new ScriptObject()
{
    class = NyanCatTouchGesture;
    TouchEventCount = 0;
    TouchEventActive[0] = false;
    TouchEventActive[1] = false;
    TouchEventActive[2] = false;
    TouchEventActive[3] = false;
    TouchEventActive[4] = false;
    LastTouchId = -1;
    LatestTouchId = -1;
};

//-----------------------------------------------------------------------------

function NyanCatTouchGesture::onTouchDownEvent( %this, %touchId, %worldPosition )
{
    //echo( "NyanCatTouchGesture::onTouchDownEvent(" @ %touchId @ "," @ %worldPosition @ ")" );

    // Sanity!
    if ( %this.TouchEventActive[%touchId] == true )
    {
        error( "NyanCatTouchGesture::onTouchDownEvent() - Touch Id already active." );
        return;        
    }

    // Calculate window position.
    %windowPosition = NyanCatWindow.getWindowPoint( %worldPosition );

    // Store the new touch position.
    %this.NewTouchPosition[%touchId] = %windowPosition;
        
    // Set the old touch position as new touch position.
    %this.OldTouchPosition[%touchId] = %windowPosition;
    
    // Flag event as active.
    %this.TouchEventActive[%touchId] = true;

    // Insert the new touch Id.
    %this.PreviousTouchId = %this.CurrentTouchId;
    %this.CurrentTouchId = %touchId;

    // Increase event count.
    %this.TouchEventCount++;
    %picked = NyanCatScene.pickPoint( %worldPosition, "", "", "Collision" );
}

//-----------------------------------------------------------------------------

function NyanCatTouchGesture::onTouchUpEvent( %this, %touchId, %worldPosition )
{
    //echo( "NyanCatTouchGesture::onTouchUpEvent(" @ %touchId @ "," @ %worldPosition @ ")" );

    // Sanity!
    if ( %this.TouchEventActive[%touchId] == false )
    {
        error( "NyanCatTouchGesture::onTouchUpEvent() - Touch Id not active." );
        return;        
    }
        
    // Reset previous touch.
    %this.OldTouchPosition[%touchId] = "";
    
    // Reset current touch.
    %this.NewTouchPosition[%touchId] = "";
    
    // Flag event as inactive.
    %this.TouchEventActive[%touchId] = false;

    // Remove the touch Id.
    if ( %this.PreviousTouchId == %touchId )
    {
         %this.PreviousTouchId = "";
    }
    if ( %this.CurrentTouchId == %touchId )
    {
         %this.CurrentTouchId = %this.PreviousTouchId;
         %this.PreviousTouchId = "";
    }

    // Decrease event count.
    %this.TouchEventCount--;
}

//-----------------------------------------------------------------------------

function NyanCatTouchGesture::onTouchDraggedEvent( %this, %touchId, %worldPosition )
{
    //echo( "NyanCatTouchGesture::onTouchDraggedEvent(" @ %touchId @ "," @ %worldPosition @ ")" );

    // Sanity!
    if ( %this.TouchEventActive[%touchId] == false )
    {
        error( "NyanCatTouchGesture::onTouchDraggedEvent() - Touch Id not active." );
        return;        
    }

    // Calculate window position.
    %windowPosition = NyanCatWindow.getWindowPoint( %worldPosition );

    // Set the current touch as the previous touch.
    %this.OldTouchPosition[%touchId] = %this.NewTouchPosition[%touchId];
    
    // Store the touch event.
    %this.NewTouchPosition[%touchId] = %windowPosition;
}

//-----------------------------------------------------------------------------

function NyanCatTouchGesture::performPanGesture( %this )
{
    // Finish if we don't have two touch events.
    if ( %this.TouchEventCount != 1 )
        return;

    // Fetch the last touch event Id.
    %touchId = %this.CurrentTouchId;

    // Sanity!
    if ( %touchId $= "" )
    {
        error( "NyanCatTouchGesture::performPanGesture() - Current touch Id not available." );
        return;
    }

    // Calculate pan offset.
    %panOffset = Vector2Sub( %this.NewTouchPosition[%touchId], %this.OldTouchPosition[%touchId] );

    // Inverse the Y offset.
    %panOffset = Vector2InverseY( %panOffset );

    // Scale the pan offset by the camera world scale.
    %panOffset = Vector2Mult( %panOffset, NyanCatWindow.getCameraWorldScale() );

    // Update the camera position.
    NyanCatWindow.setCameraPosition( Vector2Sub( NyanCatWindow.getCameraPosition(), %panOffset ) );
}

//-----------------------------------------------------------------------------

function NyanCatTouchGesture::performZoomGesture( %this )
{
    // Finish if we don't have two touch events.
    if ( %this.TouchEventCount != 2 )
        return;

    // Fetch current and previous touch Ids.
    %currentTouchId = %this.CurrentTouchId;
    %previousTouchId = %this.PreviousTouchId;

    // Finish if we don't have touch Ids active.
    if ( !%this.TouchEventActive[%currentTouchId] || !%this.TouchEventActive[%previousTouchId] )
    {
        error( "NyanCatTouchGesture::performZoomGesture() - Current or previous touch events were no active." );
        return;
    }

    %currentNewPosition = %this.NewTouchPosition[%currentTouchId];
    %currentOldPosition = %this.OldTouchPosition[%currentTouchId];
    %previousNewPosition = %this.NewTouchPosition[%previousTouchId];
    %previousOldPosition = %this.OldTouchPosition[%previousTouchId];

    // Calculate the last and current separations.
    %lastLength = Vector2Length( Vector2Abs( %currentOldPosition, %previousOldPosition ) );
    %currentLength = Vector2Length( Vector2Abs( %currentNewPosition, %previousNewPosition ) );
    
    // Calculate the change in separation length.
    %separationDelta = %currentLength - %lastLength;

    // Finish if no separation change occurred.
    if ( %separationDelta == 0 || %separationDelta $= "" )
        return;

    // Fetch the camera zoom.
    %cameraZoom =  NyanCatWindow.getCameraZoom();

    // Calculate new camera zoom.
    %newCameraZoom = %cameraZoom + ( %separationDelta * $pref::NyanCat::cameraTouchZoomRate );

    // Change the zoom.
    NyanCatWindow.setCameraZoom( %newCameraZoom ) ;
}

//-----------------------------------------------------------------------------

function NyanCat::resetManipulationModes( %this )
{
    // These control which drag modes are available or not.
    NyanCat.ManipulationModeState["off"] = true;
    NyanCat.ManipulationModeState["pan"] = false;
    NyanCat.ManipulationModeState["pull"] = false;
    
    // Set the NyanCat drag mode default.
    NyanCat.useManipulation( "off" ); 
}

//-----------------------------------------------------------------------------

function NyanCat::allowManipulation( %this, %mode )
{
    // Cannot turn-off the "off" manipulation.
    if ( %mode $= "off" )
        return;
        
    NyanCat.ManipulationModeState[%mode] = true;    
}

//-----------------------------------------------------------------------------

function NyanCat::useManipulation( %this, %mode )
{
    // Is the drag mode available?
    if ( %mode !$= "off" && !NyanCat.ManipulationModeState[%mode] )
    {
        // No, so warn.
        error( "Cannot set NyanCat drag mode to " @ %mode @ " as it is currently disabled." );
        return;
    }
    
    // Set the manipulation mode.
    NyanCat.ManipulationMode = %mode;

    // Set the current mode as text on the button.    
    if ( isObject(ManipulationModeButton) )
    {
        // Make the displayed mode more consistent.
        if ( %mode $= "off" )
            %mode = "Off";
        else if ( %mode $= "pan" )
            %mode = "Pan";
        else if ( %mode $= "pull" )
        %mode = "Pull";
        
        // Make the mode consistent when showed.
        ManipulationModeButton.Text = %mode;
    }
    
    // Reset pulled object and joint.
    NyanCat.ManipulationPullObject = "";    
    if ( NyanCat.ManipulationPullJointId !$= "" && NyanCatScene.isJoint(NyanCat.ManipulationPullJointId) )
    {
        NyanCatScene.deleteJoint( NyanCat.ManipulationPullJointId );
        NyanCat.ManipulationPullJointId = "";
    }        
}

//-----------------------------------------------------------------------------

function cycleManipulation( %make )
{
    // Finish if being released.
    if ( !%make )
        return;

    // "off" to "pan" transition.
    if ( NyanCat.ManipulationMode $= "off" )
    {
        if ( NyanCat.ManipulationModeState["pan"] )
        {
            NyanCat.useManipulation("pan");
            return;
        }
        
        NyanCat.ManipulationMode = "pan";
    }      
    
    // "pan" to "pull" transition.
    if ( NyanCat.ManipulationMode $= "pan" )
    {
        if ( NyanCat.ManipulationModeState["pull"] )
        {
            NyanCat.useManipulation("pull");
            return;
        }
            
        NyanCat.ManipulationMode = "pull";
    }

    // "pull" to "off" transition.
    if ( NyanCat.ManipulationMode $= "pull" )
    {
        NyanCat.useManipulation("off");
    }          
}

//-----------------------------------------------------------------------------

function NyanCatWindow::onTouchDown(%this, %touchID, %worldPosition)
{
    // Finish if the drag mode is off.
    if ( NyanCat.ManipulationMode $= "off" )
        return;
        
    // Set touch event.
    NyanCat.TouchController.onTouchDownEvent( %touchID, %worldPosition );
           
    // Handle "pull" mode.
    if ( NyanCat.ManipulationMode $= "pull" )
    {
        // Reset the pull
        NyanCat.ManipulationPullObject[%touchID] = "";
        NyanCat.ManipulationPullJointId[%touchID] = "";
        
        // Pick an object.
        %picked = NyanCatScene.pickPoint( %worldPosition );
        
        // Finish if nothing picked.
        if ( %picked $= "" )
            return;
        
        // Fetch the pick count.
        %pickCount = %picked.Count;
        
        for( %n = 0; %n < %pickCount; %n++ )
        {
            // Fetch the picked object.
            %pickedObject = getWord( %picked, %n );
            
            // Skip if the object is static.
            if ( %pickedObject.getBodyType() $= "static" )
                continue;
                
            // Set the pull object.
            NyanCat.ManipulationPullObject[%touchID] = %pickedObject;
            NyanCat.ManipulationPullJointId[%touchID] = NyanCatScene.createTargetJoint( %pickedObject, %worldPosition, NyanCat.ManipulationPullMaxForce );            
            return;
        }
        
        return;
    }    
}

//-----------------------------------------------------------------------------

function NyanCatWindow::onTouchUp(%this, %touchID, %worldPosition)
{
    // Finish if the drag mode is off.
    if ( NyanCat.ManipulationMode $= "off" )
        return;
        
    // Set touch event.
    NyanCat.TouchController.onTouchUpEvent( %touchID, %worldPosition );

    // Handle "pull" mode.
    if ( NyanCat.ManipulationMode $= "pull" )
    {       
        // Finish if nothing is being pulled.
        if ( !isObject(NyanCat.ManipulationPullObject[%touchID]) )
            return;
        
        // Reset the pull object.
        NyanCat.ManipulationPullObject[%touchID] = "";
        
        // Remove the pull joint.
        NyanCatScene.deleteJoint( NyanCat.ManipulationPullJointId[%touchID] );
        NyanCat.ManipulationPullJointId[%touchID] = "";        
        return;
    }      
}

//-----------------------------------------------------------------------------

function NyanCatWindow::onTouchMoved(%this, %touchID, %worldPosition)
{
    // Finish if the drag mode is off.
    if ( NyanCat.ManipulationMode $= "off" )
        return;
}

//-----------------------------------------------------------------------------

function NyanCatWindow::onTouchDragged(%this, %touchID, %worldPosition)
{
    // Finish if the drag mode is off.
    if ( NyanCat.ManipulationMode $= "off" )
        return;

    // Set touch event.
    NyanCat.TouchController.onTouchDraggedEvent( %touchID, %worldPosition );
    
    // Handle "pan" mode.
    if ( NyanCat.ManipulationMode $= "pan" )
    {
        // Fetch the touch event count.
        %touchEventCount = NyanCat.TouchController.TouchEventCount;
        
        // Do we have a single touch event?
        if ( %touchEventCount == 1 )
        {
            // Yes, so perform pan gesture.
            NyanCat.TouchController.performPanGesture();
            
            return;
        }
        
        // Do we have two event counts?
        if ( %touchEventCount == 2 )
        {
            // Yes, so perform zoom gesture.
            NyanCat.TouchController.performZoomGesture();

            return;
        }
    }
    
    // Handle "pull" mode.
    if ( NyanCat.ManipulationMode $= "pull" )
    {
        // Finish if nothing is being pulled.
        if ( !isObject(NyanCat.ManipulationPullObject[%touchID]) )
            return;
              
        // Set a new target for the target joint.
        NyanCatScene.setTargetJointTarget( NyanCat.ManipulationPullJointId[%touchID], %worldPosition );
        
        return;
    }
}

//-----------------------------------------------------------------------------

function NyanCatWindow::onMouseWheelUp(%this, %modifier, %mousePoint, %mouseClickCount)
{
    // Finish if the drag mode is not "pan".
    if ( !NyanCat.ManipulationMode $= "pan" )
        return;
        
    // Increase the zoom.
    NyanCatWindow.setCameraZoom( NyanCatWindow.getCameraZoom() + $pref::NyanCat::cameraMouseZoomRate );
}

//-----------------------------------------------------------------------------

function NyanCatWindow::onMouseWheelDown(%this, %modifier, %mousePoint, %mouseClickCount)
{
    // Finish if the drag mode is not "pan".
    if ( !NyanCat.ManipulationMode $= "pan" )
        return;

    // Increase the zoom.
    NyanCatWindow.setCameraZoom( NyanCatWindow.getCameraZoom() - $pref::NyanCat::cameraMouseZoomRate );
}
