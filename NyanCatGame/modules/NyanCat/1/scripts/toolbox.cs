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

$toyAllCategoryIndex = 1;
$defaultToySelected = false;

//-----------------------------------------------------------------------------

function ToyCategorySelectList::onSelect(%this)
{
    // Fetch the index.
    %index = %this.currentToyCategory;

    $defaultToySelected = false;

    ToyListArray.initialize(%index);
    ToyListScroller.computeSizes();
    ToyListScroller.scrollToTop();
    
    // Unload the active toy.
    unloadToy();

    // Flag as the default toy selected.
    if ($defaultToySelected)
    {
        loadToy($defaultModuleID);
    }
    else
    {
        if ( ToyListArray.getCount() > 0 )
        {
            %firstToyButton = ToyListArray.getObject(0);
            if (isObject(%firstToyButton))
                %firstToyButton.performSelect();
        }
    }
}

//-----------------------------------------------------------------------------

function ToyCategorySelectList::initialize(%this)
{
     %this.toyCategories[$toyAllCategoryIndex] = "All";
     %this.toyCategories[$toyAllCategoryIndex+1] = "Physics";
     %this.toyCategories[$toyAllCategoryIndex+2] = "Features";
     %this.toyCategories[$toyAllCategoryIndex+3] = "Stress Testing";
     %this.toyCategories[$toyAllCategoryIndex+4] = "Fun and Games";
     %this.toyCategories[$toyAllCategoryIndex+5] = "Custom";
     %this.toyCategories[$toyAllCategoryIndex+6] = "Experiments";
     %this.maxToyCategories = $toyAllCategoryIndex + 7;

     // Set the "All" category as the default.
     // NOTE:    This is important to use so that the user-configurable default toy
     //          can be selected at start-up.
     %this.currentToyCategory = $toyAllCategoryIndex;

     %this.setText("All");
     %this.onSelect();
}

//-----------------------------------------------------------------------------

function ToyCategorySelectList::nextCategory(%this)
{
    if (%this.currentToyCategory >= %this.maxToyCategories)
        return;

    %this.currentToyCategory++;

    %text = %this.toyCategories[%this.currentToyCategory];
    %this.setText(%text);
    %this.onSelect();
}

//-----------------------------------------------------------------------------

function ToyCategorySelectList::previousCategory(%this)
{
    if (%this.currentToyCategory <= $toyAllCategoryIndex)
        return;

    %this.currentToyCategory--;

    %text = %this.toyCategories[%this.currentToyCategory];
    %this.setText(%text);
    %this.onSelect();
}

//-----------------------------------------------------------------------------

function initializeToolbox()
{   
    // Populate the stock colors.
    %colorCount = getStockColorCount();
    for ( %i = 0; %i < %colorCount; %i++ )
    {
        // Fetch stock color name.
        %colorName = getStockColorName(%i);
        
        // Add to the list.        
        BackgroundColorSelectList.add( getStockColorName(%i), %i );
        
        // Select the color if it's the default one.
        if ( %colorName $= $pref::NyanCat::defaultBackgroundColor )
            BackgroundColorSelectList.setSelected( %i );
    }
    BackgroundColorSelectList.sort();

    ToyCategorySelectList.initialize();

    // Is this on the desktop?
    if ( $platform $= "windows" || $platform $= "macos" )
    {
        // Yes, so make the controls screen controls visible.
        ResolutionSelectLabel.Visible = true;
        ResolutionSelectList.Visible = true;
        FullscreenOptionLabel.Visible = true;
        FullscreenOptionButton.Visible = true;
        
        // Fetch the active resolution.
        %activeResolution = getRes();
        %activeWidth = getWord(%activeResolution, 0);
        %activeHeight = getWord(%activeResolution, 1);
        %activeBpp = getWord(%activeResolution, 2);
        
        // Fetch the resolutions.
        %resolutionList = getResolutionList( $pref::Video::displayDevice );
        %resolutionCount = getWordCount( %resolutionList ) / 3;
        %inputIndex = 0;
        %outputIndex = 0;
        for( %i = 0; %i < %resolutionCount; %i++ )
        {
            // Fetch the resolution entry.
            %width = getWord( %resolutionList, %inputIndex );
            %height = getWord( %resolutionList, %inputIndex+1 );
            %bpp = getWord( %resolutionList, %inputIndex+2 );
            %inputIndex += 3;
            
            // Skip the 16-bit ones.
            if ( %bpp == 16 )
                continue;
                
            // Store the resolution.
            $NyanCatResolutions[%outputIndex] = %width SPC %height SPC %bpp;
            
            // Add to the list.
            ResolutionSelectList.add( %width @ "x" @ %height SPC "(" @ %bpp @ ")", %outputIndex );
            
            // Select the resolution if it's the default one.
            if ( %width == %activeWidth && %height == %activeHeight && %bpp == %activeBpp )
                ResolutionSelectList.setSelected( %outputIndex );
                
            %outputIndex++;
        }
    }
    
    // Configure the main overlay.
    NyanCatWindow.add(MainOverlay);    
    %horizPosition = getWord(NyanCatWindow.Extent, 0) - getWord(MainOverlay.Extent, 0);
    %verticalPosition = getWord(NyanCatWindow.Extent, 1) - getWord(MainOverlay.Extent, 1);    
    MainOverlay.position = %horizPosition SPC %verticalPosition;    
}

//-----------------------------------------------------------------------------

function toggleToolbox(%make)
{
    // Finish if being released.
    if ( !%make )
        return;
        
    // Finish if the console is awake.
    if ( ConsoleDialog.isAwake() )
        return;       
        
    // Is the toolbox awake?
    if ( ToolboxDialog.isAwake() )
    {
        // Yes, so deactivate it.
        if ( $enableDirectInput )
            activateKeyboard();
        Canvas.popDialog(ToolboxDialog);
        MainOverlay.setVisible(1);
        return;
    }
    
    // Activate it.
    if ( $enableDirectInput )
        deactivateKeyboard();

    MainOverlay.setVisible(0);
    Canvas.pushDialog(ToolboxDialog);
}

//-----------------------------------------------------------------------------

function BackgroundColorSelectList::onSelect(%this)
{           
    // Fetch the index.
    $activeBackgroundColor = %this.getSelected();
 
    // Finish if the NyanCat scene is not available.
    if ( !isObject(NyanCatScene) )
        return;
            
    // Set the scene color.
    Canvas.BackgroundColor = getStockColorName($activeBackgroundColor);
    Canvas.UseBackgroundColor = true;
}

//-----------------------------------------------------------------------------

function ResolutionSelectList::onSelect(%this)
{
    // Finish if the NyanCat scene is not available.
    if ( !isObject(NyanCatScene) )
        return;
            
    // Fetch the index.
    %index = %this.getSelected();

    // Fetch resolution.
    %resolution = $NyanCatResolutions[%index];
    
    // Set the screen mode.    
    setScreenMode( GetWord( %resolution , 0 ), GetWord( %resolution, 1 ), GetWord( %resolution, 2 ), $pref::Video::fullScreen );
}

//-----------------------------------------------------------------------------

function PauseSceneModeButton::onClick(%this)
{
    // Sanity!
    if ( !isObject(NyanCatScene) )
    {
        error( "Cannot pause/unpause the NyanCat scene as it does not exist." );
        return;
    }
    
    // Toggle the scene pause.
    NyanCatScene.setScenePause( !NyanCatScene.getScenePause() );   
}

//-----------------------------------------------------------------------------

function ReloadToyOverlayButton::onClick(%this)
{
    // Finish if no toy is loaded.
    if ( !isObject(NyanCat.ActiveToy) )
        return;

    // Reset custom controls.
    resetCustomControls();

    // Reload the toy.
    loadToy( NyanCat.ActiveToy ); 
}

//-----------------------------------------------------------------------------

function RestartToyOverlayButton::onClick(%this)
{
    // Finish if no toy is loaded.
    if ( !isObject(NyanCat.ActiveToy) )
        return;

    // Reset the toy.
    if ( NyanCat.ActiveToy.ScopeSet.isMethod("reset") )
        NyanCat.ActiveToy.ScopeSet.reset();
}

//-----------------------------------------------------------------------------

function updateToolboxOptions()
{
    // Finish if the NyanCat scene is not available.
    if ( !isObject(NyanCatScene) )
        return;
        
    // Set the scene color.
    Canvas.BackgroundColor = getStockColorName($activeBackgroundColor);
    Canvas.UseBackgroundColor = true;        
       
    // Set option.
    if ( $pref::NyanCat::metricsOption )
        NyanCatScene.setDebugOn( "metrics" );
    else
        NyanCatScene.setDebugOff( "metrics" );

    // Set option.
    if ( $pref::NyanCat::fpsmetricsOption )
        NyanCatScene.setDebugOn( "fps" );
    else
        NyanCatScene.setDebugOff( "fps" );

    // Set option.
    if ( $pref::NyanCat::controllersOption )
        NyanCatScene.setDebugOn( "controllers" );
    else
        NyanCatScene.setDebugOff( "controllers" );
                    
    // Set option.
    if ( $pref::NyanCat::jointsOption )
        NyanCatScene.setDebugOn( "joints" );
    else
        NyanCatScene.setDebugOff( "joints" );

    // Set option.
    if ( $pref::NyanCat::wireframeOption )
        NyanCatScene.setDebugOn( "wireframe" );
    else
        NyanCatScene.setDebugOff( "wireframe" );
        
    // Set option.
    if ( $pref::NyanCat::aabbOption )
        NyanCatScene.setDebugOn( "aabb" );
    else
        NyanCatScene.setDebugOff( "aabb" );

    // Set option.
    if ( $pref::NyanCat::oobbOption )
        NyanCatScene.setDebugOn( "oobb" );
    else
        NyanCatScene.setDebugOff( "oobb" );
        
    // Set option.
    if ( $pref::NyanCat::sleepOption )
        NyanCatScene.setDebugOn( "sleep" );
    else
        NyanCatScene.setDebugOff( "sleep" );                

    // Set option.
    if ( $pref::NyanCat::collisionOption )
        NyanCatScene.setDebugOn( "collision" );
    else
        NyanCatScene.setDebugOff( "collision" );
        
    // Set option.
    if ( $pref::NyanCat::positionOption )
        NyanCatScene.setDebugOn( "position" );
    else
        NyanCatScene.setDebugOff( "position" );
        
    // Set option.
    if ( $pref::NyanCat::sortOption )
        NyanCatScene.setDebugOn( "sort" );
    else
        NyanCatScene.setDebugOff( "sort" );        
                          
    // Set the options check-boxe.
    MetricsOptionCheckBox.setStateOn( $pref::NyanCat::metricsOption );
    FpsMetricsOptionCheckBox.setStateOn( $pref::NyanCat::fpsmetricsOption );
    ControllersOptionCheckBox.setStateOn( $pref::NyanCat::controllersOption );
    JointsOptionCheckBox.setStateOn( $pref::NyanCat::jointsOption );
    WireframeOptionCheckBox.setStateOn( $pref::NyanCat::wireframeOption );
    AABBOptionCheckBox.setStateOn( $pref::NyanCat::aabbOption );
    OOBBOptionCheckBox.setStateOn( $pref::NyanCat::oobbOption );
    SleepOptionCheckBox.setStateOn( $pref::NyanCat::sleepOption );
    CollisionOptionCheckBox.setStateOn( $pref::NyanCat::collisionOption );
    PositionOptionCheckBox.setStateOn( $pref::NyanCat::positionOption );
    SortOptionCheckBox.setStateOn( $pref::NyanCat::sortOption );
    BatchOptionCheckBox.setStateOn( NyanCatScene.getBatchingEnabled() );
    
    // Is this on the desktop?
    //if ( $platform $= "windows" || $platform $= "macos" )
    if ( $platform !$= "iOS" )
    {
        // Set the fullscreen check-box.
        FullscreenOptionButton.setStateOn( $pref::Video::fullScreen );
        
        // Set the full-screen mode appropriately.
        if ( isFullScreen() != $pref::Video::fullScreen )
            toggleFullScreen();            
    }  
}

//-----------------------------------------------------------------------------

function setFullscreenOption( %flag )
{
    $pref::Video::fullScreen = %flag;
    updateToolboxOptions();
}

//-----------------------------------------------------------------------------

function setMetricsOption( %flag )
{
    $pref::NyanCat::metricsOption = %flag;
    updateToolboxOptions();
}

//-----------------------------------------------------------------------------

function setFPSMetricsOption( %flag )
{
    $pref::NyanCat::fpsmetricsOption = %flag;
    updateToolboxOptions();
}

//-----------------------------------------------------------------------------

function setMetricsOption( %flag )
{
    $pref::NyanCat::metricsOption = %flag;
    updateToolboxOptions();
}

//-----------------------------------------------------------------------------

function setControllersOption( %flag )
{
    $pref::NyanCat::controllersOption = %flag;
    updateToolboxOptions();
}

//-----------------------------------------------------------------------------

function setJointsOption( %flag )
{
    $pref::NyanCat::jointsOption = %flag;
    updateToolboxOptions();
}

//-----------------------------------------------------------------------------

function setWireframeOption( %flag )
{
    $pref::NyanCat::wireframeOption = %flag;
    updateToolboxOptions();
}

//-----------------------------------------------------------------------------

function setAABBOption( %flag )
{
    $pref::NyanCat::aabbOption = %flag;
    updateToolboxOptions();
}

//-----------------------------------------------------------------------------

function setOOBBOption( %flag )
{
    $pref::NyanCat::oobbOption = %flag;
    updateToolboxOptions();
}

//-----------------------------------------------------------------------------

function setSleepOption( %flag )
{
    $pref::NyanCat::sleepOption = %flag;
    updateToolboxOptions();
}

//-----------------------------------------------------------------------------

function setCollisionOption( %flag )
{
    $pref::NyanCat::collisionOption = %flag;
    updateToolboxOptions();
}

//-----------------------------------------------------------------------------

function setPositionOption( %flag )
{
    $pref::NyanCat::positionOption = %flag;
    updateToolboxOptions();
}

//-----------------------------------------------------------------------------

function setSortOption( %flag )
{
    $pref::NyanCat::sortOption = %flag;
    updateToolboxOptions();
}

//-----------------------------------------------------------------------------

function ToyListScroller::scrollToNext(%this)
{
    %currentScroll = %this.getScrollPositionY();
    %currentScroll += 85;
    %this.setScrollPosition(0, %currentScroll);
}

//-----------------------------------------------------------------------------

function ToyListScroller::scrollToPrevious(%this)
{
    %currentScroll = %this.getScrollPositionY();
    %currentScroll -= 85;
    %this.setScrollPosition(0, %currentScroll);
}

//-----------------------------------------------------------------------------

function ToyListArray::initialize(%this, %index)
{
    %this.clear();
    %currentExtent = %this.extent;
    %newExtent = getWord(%currentExtent, 0) SPC "20";
    %this.Extent = %newExtent;
    
    // Fetch the toy count.
    %toyCount = NyanCatToys.getCount();

    // Populate toys in the selected category.
    for ( %toyIndex = 0; %toyIndex < %toyCount; %toyIndex++ )
    {
        // Fetch the toy module.
        %moduleDefinition = NyanCatToys.getObject( %toyIndex );
        
        // Skip the toy module if the "all" category is not selected and if the toy is not in the selected category.
        if ( %index != $toyAllCategoryIndex && %moduleDefinition.ToyCategoryIndex != %index )
            continue;

        // Fetch the module version.
        %versionId = %moduleDefinition.versionId;

        // Format module title so that version#1 doesn't show version but all other versions do.
        if ( %versionId == 1 )
            %moduleTitle = %moduleDefinition.moduleId;
        else
            %moduleTitle = %moduleDefinition.moduleId SPC "(v" @ %moduleDefinition.versionId @ ")";

        // Add the toy GUI list.
        %this.addToyButton(%moduleTitle, %moduleDefinition);
        
        // Select the toy if it's the default and we've not selected a toy yet.
        if (!$defaultToySelected && %moduleDefinition.moduleId $= $pref::NyanCat::defaultToyId && %moduleDefinition.versionId == $pref::NyanCat::defaultToyVersionId )
        {
            $defaultToySelected = true;
            $defaultModuleID = %moduleDefinition.getId();
        }
    }
}

//-----------------------------------------------------------------------------

function ToyListArray::addToyButton(%this, %moduleTitle, %moduleDefintion)
{
    %button = new GuiButtonCtrl()
    {
        canSaveDynamicFields = "0";
        HorizSizing = "relative";
        class = "ToySelectButton";
        VertSizing = "relative";
        isContainer = "0";
        Profile = "BlueButtonProfile";
        toolTipProfile = "GuiToolTipProfile";
        toolTip = %moduleDefintion.Description;
        Position = "0 0";
        Extent = "160 80";
        Visible = "1";
        toy = %moduleDefintion.getId();
        isContainer = "0";
        Active = "1";
        text = %moduleTitle;
        groupNum = "-1";
        buttonType = "PushButton";
        useMouseEvents = "0";
    };
     
    %button.command = %button @ ".performSelect();";
    
    %this.add(%button);
}

//-----------------------------------------------------------------------------

function ToySelectButton::performSelect(%this)
{
    // Finish if already selected.
    if ( %this.toy == NyanCat.ActiveToy )
        return;
    
    // Load the selected toy.
    loadToy( %this.toy );
}
