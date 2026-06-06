// These methods are to overcome the fact that particle emitters are 
// scene objects that must move with the main game sprite.

function PlayerSprite::updatePosition(%this, %position)
{
    %this.setPosition(%this.position.x, %position);
    %this.updateEmitterPositions(%this.position);
}

// This sets the velocities of all attached emitters to match
// the game sprite
function PlayerSprite::updateVelocity(%this, %velocity)
{
    %this.setLinearVelocity(%velocity);
    %this.redEmitter.setLinearVelocity(%this.getLinearVelocity());
    %this.orangeEmitter.setLinearVelocity(%this.getLinearVelocity());
    %this.yellowEmitter.setLinearVelocity(%this.getLinearVelocity());
    %this.greenEmitter.setLinearVelocity(%this.getLinearVelocity());
    %this.blueEmitter.setLinearVelocity(%this.getLinearVelocity());
    %this.purpleEmitter.setLinearVelocity(%this.getLinearVelocity());
}

// This updates the positions of all attached emitters to match
// the game sprite.  Note that the NyanCatBehavior breaks all of the
// weld joints before this and needs to regenerate the joints after.
function PlayerSprite::updateEmitterPositions(%this, %target)
{
    if( !%this.getFlipX() )
    {
        %position = Vector2Add(%target, "1.0 0.3");
        %this.redEmitter.setPosition(%position);
        %position = Vector2Add(%target, "1.0 0.15");
        %this.orangeEmitter.setPosition(%position);
        %position = Vector2Add(%target, "1.0 0.05");
        %this.yellowEmitter.setPosition(%position);
        %position = Vector2Add(%target, "1.0 -0.05");
        %this.greenEmitter.setPosition(%position);
        %position = Vector2Add(%target, "1.0 -0.15");
        %this.blueEmitter.setPosition(%position);
        %position = Vector2Add(%target, "1.0 -0.3");
        %this.purpleEmitter.setPosition(%position);
    }
    else
    {
        %position = Vector2Add(%target, "-1.0 0.3");
        %this.redEmitter.setPosition(%position);
        %position = Vector2Add(%target, "-1.0 0.15");
        %this.orangeEmitter.setPosition(%position);
        %position = Vector2Add(%target, "-1.0 0.05");
        %this.yellowEmitter.setPosition(%position);
        %position = Vector2Add(%target, "-1.0 -0.05");
        %this.greenEmitter.setPosition(%position);
        %position = Vector2Add(%target, "-1.0 -0.15");
        %this.blueEmitter.setPosition(%position);
        %position = Vector2Add(%target, "-1.0 -0.3");
        %this.purpleEmitter.setPosition(%position);
    }
}
