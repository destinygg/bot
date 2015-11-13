First Header  | Second Header
------------- | -------------
Content Cell  | Content Cell
Content Cell  | Content Cell

# Bot

Source code for Bot of [Destiny.gg chat](http://www.destiny.gg/embed/chat)

## User Guide

### How to Get Muted or Banned: An Incomplete List
* Use full width characters
* Say words on the mute or ban word list
* Say something that is very similar to what someone else previously said
* Say something that is very similar to what you said previously
* Say more than 5 consecutive numbers in chat
* Use more than 7 emotes in a line

### Commands
| Commands                                     | Result
| ---------------------------------------------|----------
| !playlist                                    | Outputs a link to Destiny's Last.fm
| !rules                                       | Outputs a link to this page
| !sponsor                                     | Outputs links to Destiny's sponsors
| !irc                                         | Outputs a link to qchat.rizon.net/?channels=#destinyecho
| !time                                        | Outputs time in Destiny's time zone
| !live                                        | Outputs the stream status
| !starcraft                                   | Outputs information on Destiny's latest StarCraft match
| !song                                        | Outputs the most recently scrobbled song
| !pastsong                                    | Outputs the second most recently scrobbled song
| !twitter                                     | Outputs Destiny's latest tweet
| !youtube                                     | Outputs a link to Destiny's latest YouTube upload
| !blog                                        | Outputs a link to Destiny's latest blog post

### Moderator Only Commands
| Mute/Ban Commands                            | Result
| ---------------------------------------------|-----
| !AddMute 10m any phrase                      | Automatically mutes users who say `any phrase` (case insensitive) for 10m\*†
| !AddBan 10m any phrase                       | Automatically bans‡ users who say `any phrase` (case insensitive) for 10m\*†
| !AddMuteRegex 10m \s                         | Automatically mutes users who say any whitespace character§ for 10m\*†
| !AddBanRegex 10m \s                          | Automatically bans‡ users who say any whitespace character§ for 10m\*†
| !DeleteMute any phrase                       | Removes the automatic mute on `any phrase`
| !DeleteBan any phrase                        | Removes the automatic ban on `any phrase`
| !DeleteMuteRegex 10m \s                      | Removes the automatic mute on any whitespace character
| !DeleteBanRegex 10m \s                       | Removes the automatic ban on any whitespace character
| !Mute MyUser 10m Reason                      | Mutes MyUser with the optional `Reason` for 10m\*
| !Ban MyUser 10m Reason                       | Bans MyUser with the optional `Reason` for 10m\*
| !Ipban MyUser 10m Reason                     | Ipbans MyUser with the optional `Reason` for 10m\*
| !Unmute MyUser <br> !Unban MyUser            | Unbans MyUser (this also removes mutes)
| !Nuke 10m any phrase                         | Mutes users who said `any phrase` before or after the nuke is launched for 10m\*
| !NukeRegex 10m \s                            | Mutes users who said any whitespace character§ before or after the nuke is launched for 10m\*
| !Aegis                                       | Cancels all nukes and unmutes everyone muted by a nuke

\* All times are optional. In most cases, if a number is not given the Bot will assume 10, and if a unit is not given the Bot will assume minutes. In other words, `!ban1` yields a 1m ban, `!band` yields a 10 day ban, and `!ban` yields a 10m ban. The exception is `!ipban`, which defaults to permanent if a time is not given. Supported units include: s, sec, secs, second, seconds, m, min, mins, minute, minutes, h, hr, hrs, hour, hours, d, day, days, p, per, perm, and permanent.

† The 10m only applies for the first infraction; the time will double for each subsequent infraction.

‡ It is highly recommended that you use the automatic mutes instead of the automatic bans. Bans prevent users from seeing chat and should be reserved for the very worst offenses.

§ Regex has no options enabled; for case insensitivity use [modes](http://www.regular-expressions.info/modifiers.html) like `(?i:cAsE dOeSn'T mAtTeR)`. This makes Regex Mutes/Bans ideal for prohibiting things like YouTube or Imgur link IDs.

| Other Moderator Only Commands                | Result
| ---------------------------------------------|-
| !Sing                                        | I sing!
| !Dance                                       | I dance!
| !AddEmote MyEmote                            | Adds `MyEmote` to the third party emotes list, subjecting it to the same rules as native emotes
| !DeleteEmote MyEmote                         | Removes `MyEmote` from the third party emotes list
| !ListEmotes                                  | Outputs all third party emotes
| !AddCommand !Trigger I'm triggered           | Anyone saying `!Trigger` will evoke the output `I'm triggered`
| !DeleteCommand !Trigger                      | Removes !Trigger from Bot's commands
| !Sub On                                      | Enables subscribers only mode
| !Sub Off                                     | Disables subscribers only mode
| !Stalk MyUser                                | Outputs MyUser's last line and its timestamp

Moderators may `/msg` the Bot with Commands, and the Bot will `/msg` back the output.

Many commands have aliases. For example, `!i MyUser` will permanently ban MyUser's IP. See `Commander.cs` and `ModCommander.cs` for a complete listing.

## Technical Guide

### Building

In `\Dbot.Utility\PrivateConstants.cs.template` insert your own keys, then rename the file to `PrivateConstants.cs`.

The executable expects a SQLite Database named `DbotDB.sqlite` to be in the same directory as itself. A database with the necessary schema may be found at `\Dbot.Data\DbotDB.sqlite`.

### Testing

The root directory contains `sqlite.testsettings`. Load it before running tests!

### Pull requests

Submit SQLite schema changes to `DharmaTurtle`.

If you add a command, add a test for it demonstrating functionality.

This is run in Linux, so make sure everything remains Mono compatible.