# torque2d-nyancat
A simple click-the-moving-thing game, NyanCat-themed.

I originally wrote this game in Torque 2D ( https://torque3d.org/torque2D ) in about three hours at a Border's Book Store in Las Vegas as a walkthrough of how to create a simple game.

This distribution runs on Windows.  It should work on other platforms if run with the requisite Torque 2D release package.

Note that this project contains a lot of "default" assets that could be removed to save space.  Specifically, the ToyAssets module is probably entirely superfluous.  I just took a template project and went from there.

# NyanCat.wav
Obviously not mine. See LICENSE file for attribution.

# Nyan cat sprite image
I added a second frame with the "waves" in the rainbow portion inverted.  See LICENSE file for attribution.

# main menu image
The main menu image is from https://www.deviantart.com/studiomarimo/art/Nyan-nyan-nyanyanyanyan-253160677.  See LICENSE file
for attribution.

To play, just pull this repo, change to the NyanCatGame folder, and run the NyanCatRevisited executable. https://youtu.be/ytVUGj9uZJk?si=7vNxZsEbNFhPihVV shows how to launch the game in Love2D, (which you can ignore) and basic gameplay.

Clicking NyanCat will reverse its direction and teleport it vertically to a random location, adding one point to the score. If NyanCat escapes off of either edge the score is reduced by one point. There is no lose condition: the score will just drop to zero.  As an excersise, you can have the game exit to the main menu when the score drops below 0.
