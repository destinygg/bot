# Bot

Source code for Bot of [Destiny.gg chat](http://www.destiny.gg/embed/chat)

## Building

In `\Dbot.Utility\PrivateConstants.cs.template` insert your own keys, then rename the file to `PrivateConstants.cs`.

The executable expects a SQLite Database named `DbotDB.sqlite` to be in the same directory as itself. A database with the necessary schema may be found at `\Dbot.Data\DbotDB.sqlite`.

## Testing

The root directory contains `sqlite.testsettings`. Load it before running tests!

## Pull requests

Submit SQLite schema changes to `DharmaTurtle`.

If you add a command, add a test for it demonstrating functionality.

This is run in Linux, so make sure everything remains Mono compatible.