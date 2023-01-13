# Dialog layout
This contains a sample dialog along with descriptions of the sections and subsections of a conversation file.


```json
{
	"metadata":
	{
		"version":1
	},
	"lines":
	[
		{
			"speaker":"npc",
			"label":"start",
			"message":"playerGreetedStart",
			"text":"Hope you're having a good day!",
			"jump":null,
			"options":[],
			"notes":null
		},
		{
			"speaker":"player",
			"label":null,
			"message":null,
			"text":"Choose an option",
			"jump":null,
			"options":
			[
				{
					"text":"Sorry, what was that? (restart)",
					"message":"greetingRestarted",
					"jump":"start"
				},
				{
					"text":"Thanks! Have a good one! (end the conversation)",
					"message":null,
					"jump":"end"
				}
			],
			"notes":"The player gets to choose this one, this can loop forever. The jump option for end isn't necessary, but is more explicit this way."
		}
	],
	"end":
	{
		"message":"playerGreetedEnd"
	}
}
```

## `metadata` (Object) (REQUIRED)
This contains information about the file on the whole. If the conversations become too bloated, separating this into a separate file would make it a faster parse and wouldn't require loading the whole conversation until needed. Would be a breaking change to make this split, but that's acceptable since the field could be ignored during an upgrade.

### `version` (Number) (Required)
- Serialization format version for automatic upgrades as necessary



## `lines` (Array) (REQUIRED)
This is the primary conversation block, and contains the text of the chat/dialog between the player and a character. The first entry is run by default.

### `speaker` (String) (Optional)
- A string key that can be used externally to determine visuals, etc.
- Null / Empty string is a valid speaker, but the field is required. The suggested default use is for a narrator or narrator's eye view. Up to interpretation by user.

### `label` (String) (Optional)
- A C style label that can be looked up by jump for branching conversations. Acts as a jump point for navigating the conversation from other options.
- An empty label is ignored / has no data tracked.
- The only reserved label is `end`. 

### `message` (String) (Optional) 
- A string used as an argument to a generic message system callback. Can be connected to and interpreted as user desires.
- Empty String or Null will result in no message dispatch. 

### `text` (String) (REQUIRED)
- The one and only required field, everything else has reasonable defaults. This is the text displayed. This must contain data, even if it's empty. It cannot be null, and is assumed not to be null.
- If the `options` list is present but this is an empty string, nothing is displayed to the player exept options. Otherwise, this text appears as a pre-option block for use however the user desires.

### `jump` (String) (Optional)
- Leaving "jump" empty will cascade down to the next entry in the `conversation` list upon continuing.
- If there is nothing to proceed to and nothing specified further in the `conversation` list, then the conversation ends. This applies to both empty string or null. 
- Specifying "end" inside `jump` will stop the conversation immediately and dispatch the end block.

### `options` (Array) (Optional)
- Series of player-facing options that block until one of them is selected. Once selected, the data can be interpreted.
- Array of objects containing text, message, and jump entries. Purpose of text, message, and jump are identical to as they are described above.
- Can be null or empty, and often will be. When null or with no contents, nothing is shown for the player to choose from.
- If present, player must select an option before continuing the conversation. If not present, the one and only player interaction will continue the conversation.

### `notes` (String) (Optional)
- Largely unused and for the purpose of adding notes about this particular piece of dialog, etc.



## `end` (Object) (REQUIRED)
This requiored contains an optioanl string message that fires once the conversation is completely finished. A start doesn't exist because the first item is what's run when a conversation begins. Even if empty, this must be present.

### `message` (String) (Optional)
- A string used as an argument to a generic message system callback. Can be connected to and interpreted as user desires.
- Empty String or Null will result in no message dispatch. 
- Is not differentiated from `message` from within conversation list or options sublist.