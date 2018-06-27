using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_Master_Teemo_Bot {

    /// <summary>
    /// Holds the defining configuration properties of the bot.
    /// 
    /// This includes the: name, private token, version and command prefixes.
    /// The format is in json format.
    /// 
    /// eg.
    /// 
    /// {
    ///     "Name": "The name of the bot",
    ///     "Token": "Your private token.somethingsomething",
    ///     "Version": "1.something omega",
    ///     "CommandPrefixes": ["cmdprefix!", "anothercmdprefix", "and another :) "]
    /// }
    /// </summary>
    class BotDefinitions {
        
        /// <summary>
        /// The display name of the bot. 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The uniuqe private token for the discord bot.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Bot version.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// The prefix or prefixes put at the beginning of a message
        /// to denote a command for this bot.
        /// </summary>
        public string[] CommandPrefixes { get; set; }
    }
}
