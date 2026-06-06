if(!isObject(NyanCatBehavior))
{
    %template = new BehaviorTemplate(NyanCatBehavior);
    %template.friendlyName = "Input Target";
    %template.behaviorType = "Input";
    %template.description = "Adds an input responder to the collision area of the owner of the behavior";

    // Fields
    %template.addBehaviorField(speed, "The current speed of our NyanCat", "float", 2.0);

    // Outputs
    %template.addBehaviorOutput(inputDown, "Input Down Event", "Signals that a down event has occurred");
    %template.addBehaviorOutput(inputUp, "Input Up Event", "Signals that an up event has occured");
    %template.addBehaviorOutput(inputDrag, "Input Drag Event", "Signals that a drag event has occured");
}

/// <summary>
/// Called when behavior is added to an object.
/// </summary>
function NyanCatBehavior::onBehaviorAdd(%this)
{
    //TODO: make sure the owner reacts to touch events on its collision area
    %this.owner.setUseInputEvents(true);
}

/// <summary>
/// Called when the behavior is added to the scene
/// </summary>
/// <param name="scene">The scene object the behavior was added to.</param>
function NyanCatBehavior::onAddToScene(%this, %scene)
{
    //echo(" --- NyanCatBehavior::onAddToScene()");
    %this.scene = %scene;
    
    if( !%this.added )
    {
        %this.added = true;

        // Nyan Cat Tail
        %this.owner.redEmitter = %this.createTailEmitter(-1, 0.3, "RedStarParticles");
        %this.owner.orangeEmitter = %this.createTailEmitter(-1, 0.15, "OrangeStarParticles");
        %this.owner.yellowEmitter = %this.createTailEmitter(-1, 0.05, "YellowStarParticles");
        %this.owner.greenEmitter = %this.createTailEmitter(-1, -0.05, "GreenStarParticles");
        %this.owner.blueEmitter = %this.createTailEmitter(-1, -0.15, "BlueStarParticles");
        %this.owner.purpleEmitter = %this.createTailEmitter(-1, -0.3, "PurpleStarParticles");

        %this.flipEmitters();

        %this.setVelocity();
    }
}

function NyanCatBehavior::createTailEmitter(%this, %x, %y, %name)
{
    // Create a particle player
    %particlePlayer = new ParticlePlayer();
    %particlePlayer.BodyType = dynamic;
    %particlePlayer.SetSize(NyanCat.CameraWidth, NyanCat.CameraHeight);
    %particlePlayer.SceneLayer = NyanCat.ForegroundDomain;
    %particlePlayer.ParticleInterpolation = true;
    %particlePlayer.Particle = "NyanCatAssets:" @ %name;
    %particlePlayer.SizeScale = "1.0";
    %particlePlayer.EmissionRateScale = "1.0";
    %particlePlayer.ForceScale = "0.0";
    %particlePlayer.CameraIdleDistance = NyanCat.CameraWidth * 1.0;
    %this.scene.add( %particlePlayer ); 
    return %particlePlayer;    
}

/// <summary>
/// Handles touch down event and raises an output signal.
/// </summary>
function NyanCatBehavior::onTouchDown(%this, %touchID, %worldPos)
{
    //echo(" --- NyanCatBehavior::onTouchDown()");
    %this.reverseDirection();
    NyanCat.score++;
    %this.speed += 0.25;
    NyanCatScore.text = NyanCat.score;
    // Raise output event
    %this.owner.Raise(%this, inputDown);
}

/// <summary>
/// Handles touch down event and raises an output signal.
/// </summary>
function NyanCatBehavior::onCollision(%this, %colObjects, %colData)
{
    %count = getWordCount(%colObjects);
    for( %i = 0; %i < %count; %i++ )
    {
        %obj = getWord(%colObjects, %i);
        if( %obj == NyanCatWorldLimits.getId() )
        {
            %this.setVelocity();
        }
    }
}

function NyanCatBehavior::onEndCollision(%this, %touchID, %worldPos)
{
    //echo(" -- NyanCatBehavior::onEndCollision()");
    %this.reverseDirection();
    if( NyanCat.score > 0 )
        NyanCat.score--;
    NyanCatScore.text = NyanCat.score;
}

function NyanCatBehavior::reverseDirection(%this)
{
    %range = (NyanCatWorldLimits.size.y - 1) / 2;
    %newYPosition = getRandom() * %range;
    %coinFlip = getRandom(0, 1);
    if( !%coinFlip )
        %newYPosition = %newYPosition * -1;

    // flip sprite.
    %this.owner.setFlipX(!%this.owner.getFlipX());

    // Break weld joints so that the owner can be warped
    // to its new location
    %this.clearEmitters();
    
    %this.owner.updatePosition(%newYPosition);
    
    %this.setVelocity();

    // switch emitters to other side and recreate all 
    // weld joints
    %this.flipEmitters();
}

function NyanCatBehavior::setVelocity(%this)
{
    if( %this.owner.getFlipX() )
        %velocity = -1 * %this.speed;
    else
        %velocity = %this.speed;
    %this.owner.updateVelocity(%velocity SPC 0);
}

function NyanCatBehavior::clearEmitters(%this)
{
    %this.scene.deleteJoint(%this.owner.redJoint);
    %this.scene.deleteJoint(%this.owner.orangeJoint);
    %this.scene.deleteJoint(%this.owner.yellowJoint);
    %this.scene.deleteJoint(%this.owner.greenJoint);
    %this.scene.deleteJoint(%this.owner.blueJoint);
    %this.scene.deleteJoint(%this.owner.purpleJoint);
}

function NyanCatBehavior::flipEmitters(%this)
{
    if( %this.owner.getFlipX() )
    {
        %this.owner.redJoint = %this.scene.createDistanceJoint(%this.owner, %this.owner.redEmitter, 1, 0.3, 0.0, 0.0, false);
        %this.owner.orangeJoint = %this.scene.createDistanceJoint(%this.owner, %this.owner.orangeEmitter, 1, 0.15, 0.0, 0.0, false);
        %this.owner.yellowJoint = %this.scene.createDistanceJoint(%this.owner, %this.owner.yellowEmitter, 1, 0.05, 0.0, 0.0, false);
        %this.owner.greenJoint = %this.scene.createDistanceJoint(%this.owner, %this.owner.greenEmitter, 1, -0.05, 0.0, 0.0, false);
        %this.owner.blueJoint = %this.scene.createDistanceJoint(%this.owner, %this.owner.blueEmitter, 1, -0.15, 0.0, 0.0, false);
        %this.owner.purpleJoint = %this.scene.createDistanceJoint(%this.owner, %this.owner.purpleEmitter, 1, -0.3, 0.0, 0.0, false);
    }
    else
    {
        %this.owner.redJoint = %this.scene.createDistanceJoint(%this.owner, %this.owner.redEmitter, -1, 0.3, 0.0, 0.0, false);
        %this.owner.orangeJoint = %this.scene.createDistanceJoint(%this.owner, %this.owner.orangeEmitter, -1, 0.15, 0.0, 0.0, false);
        %this.owner.yellowJoint = %this.scene.createDistanceJoint(%this.owner, %this.owner.yellowEmitter, -1, 0.05, 0.0, 0.0, false);
        %this.owner.greenJoint = %this.scene.createDistanceJoint(%this.owner, %this.owner.greenEmitter, -1, -0.05, 0.0, 0.0, false);
        %this.owner.blueJoint = %this.scene.createDistanceJoint(%this.owner, %this.owner.blueEmitter, -1, -0.15, 0.0, 0.0, false);
        %this.owner.purpleJoint = %this.scene.createDistanceJoint(%this.owner, %this.owner.purpleEmitter, -1, -0.3, 0.0, 0.0, false);
    }
}
