using LiveSplit.Model;
using LiveSplit.UI.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: ComponentFactory(typeof(SpeechFactory))]

namespace LiveSplit.UI.Components
{
    public class SpeechFactory : IComponentFactory
    {
        public string ComponentName
        {
            get { return "Speech"; }
        }

        public string Description
        {
            get { return "Speaks to you about certain information of the splits."; }
        }

        public ComponentCategory Category
        {
            get { return ComponentCategory.Media; }
        }

        public IComponent Create(LiveSplitState state)
        {
            return new SpeechComponent(state);
        }

        public string UpdateName
        {
            get { return ComponentName; }
        }

        public string XMLURL
        {
#if RELEASE_CANDIDATE
            get { return "http://livesplit.org/update_rc_sdhjdop/Components/update.LiveSplit.Speech.xml"; }
#else
            get { return "http://livesplit.org/update/Components/update.LiveSplit.Speech.xml"; }
#endif
        }

        public string UpdateURL
        {
#if RELEASE_CANDIDATE
            get { return "http://livesplit.org/update_rc_sdhjdop/"; }
#else
            get { return "http://livesplit.org/update/"; }
#endif
        }

        public Version Version
        {
            get { return Version.Parse("1.0.0"); }
        }
    }
}
