# Dialog layout
This contains a sample dialog along with descriptions of the sections and subsections of a conversation file.


```json
"metadata":
{
	"version":1,
},
"conversation":
[
	{
		"speaker":"npc",
		"id":"start",
		"event":"playerGreetedStart",
		"text":"Hope you're having a good day!",
		"goto":null,
		"options":[],
		"notes":null
	},
	{
		"speaker":"player",
		"id":null,
		"event":null,
		"text":"Choose an option",
		"goto":null,
		"options":
		[
			{
				"text":"Sorry, what was that? (restart)",
				"event":"greetingRestarted",
				"goto":"start",
			},
			{
				"text":"Thanks! Have a good one! (end the conversation)",
				"event":null,
				"goto":"end",
			}
		],
		"notes":"The player gets to choose this one, this can loop forever. the goto option for end isn't necessary, but is more explicit this way."
	}
],
"end":
{
	"event":"playerGreetedEnd"
}
```

## `metadata` (Object) (REQUIRED)
This contains information about the file on the whole. If the conversations become too bloated, separating this into a separate file would make it a faster parse and wouldn't require loading the whole conversation until needed. Would be a breaking change to make this split, but that's acceptable since the field could be ignored during an upgrade.

### `version` (Number) (Required)
- Serialization format version for automatic upgrades as necessary



## `conversation` (Array) (REQUIRED)
This is the primary conversation block, and contains the text of the chat/dialog between the player and a character. The first entry is run by default.

### `text` (String) (REQUIRED)
- The one and only required field, everything else has reasonable defaults. This is the text displayed. This must contain data, even if it's empty. It cannot be null, and is assumed not to be null.
- In the event `options` list is present but this is an empty string, nothing is displayed to the player exept options. Otherwise, this text appears as a pre-option block for use however the user desires.

### `id` (String) (Optional)
- ID that can be looked up by goto for branching conversations. Acts as a jump point for navigating the conversation from other options.
- An empty ID is ignored
- The only reserved ID is `end`. 

### `speaker` (String) (Optional)
- A string key that can be used externally to determine visuals, etc.
- Null / Empty string is a valid speaker, but the field is required. The suggested default use is for a narrator or narrator's eye view. Up to interpretation by user.

### `event` (String) (Optional) 
- A string used as an argument to a generic event system callback. Can be connected to and interpreted as user desires.
- Empty String or Null will result in no event dispatch. 

### `goto` (String) (Optional)
- Leaving "goto" empty will cascade down to the next entry in the `conversation` list upon continuing.
- If there is nothing to proceed to and nothing specified further in the `conversation` list, then the conversation ends. This applies to both empty string or null. 
- Specifying "end" inside `goto` will stop the conversation immediately and dispatch the end block.

### `options` (Array) (Optional)
- Series of player-facing options that block until one of them is selected. Once selected, the data can be interpreted.
- Array of objects containing text, event, and goto entries. Purpose of text, event, and goto are identical to as they are described above.
- Can be null or empty, and often will be. When null or with no contents, nothing is shown for the player to choose from.
- If present, player must select an option before continuing the conversation. If not present, the one and only player interaction will continue the conversation.

### `notes` (String) (Optional)
- Largely unused and for the purpose of adding notes about this particular piece of dialog, etc.



## `end` (Object) (REQUIRED)
This contains an optioanl string event that fires once the conversation is completely finished. A start doesn't exist because the first item is what's run when a conversation begins.

