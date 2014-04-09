Minesweeper
===========
-This project is an implementation of popular minesweeper game provided by Microsoft.

-In this four activities have been made-
 1)Mainpage
 2)Game
 3)Rules
 4)About
 
-Every activity has two associated files, .xaml and .cs.

-.xaml is used for as presentation logic and .xs is used as business logic.

-Game's working can be explained as following-

... Custom sized grid of buttons is generated and placed in gridpanel dynamically.

... then random number of mines(represented as ASTERISK) is placed at random cells and a list of all mine positions is maintained.

... then each cell is given a number based on number of mines in its adjacent cells.

... thus grid(or board) is filled and numbers are hidden.

... when user clicks a cell, its text value is checked, if it is a mine(represented as ASTERISK), game is over and user is notified.

... if it is a number then adjacent cells are opened recursively and if all cells have been opened, user wins otherwise the game continues.

... timer is set such that it goes off on the first click.
