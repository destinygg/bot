# Bot

Source code for Bot of [Destiny.gg chat](http://www.destiny.gg/embed/chat)

## Building

In `\Dbot.Utility\PrivateConstants.cs.template` insert your own keys, then rename the file to `PrivateConstants.cs`.

The executable expects the SQLite Database `DbotDB.sqlite` to be in the same directory as itself. A database with the necessary schema may be found in `\Dbot.Data\DbotDB.sqlite.template`.

## Testing

The root directory contains `sqlite.testsettings`. Load it before running tests!

## Pull requests

Any SQLite schema changes should be reflected in the `DbotDB.sqlite.template`.

If you add a command, add a test for it demonstrating functionality.

This is run in Linux, so make sure everything remains Mono compatible.