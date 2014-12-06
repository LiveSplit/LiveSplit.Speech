using LiveSplit.Model;
using LiveSplit.Model.Comparisons;
using LiveSplit.TimeFormatters;
using LiveSplit.UI.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Media;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiveSplit.UI.Components
{
    public class SpeechComponent : LogicComponent, IDeactivatableComponent
    {
        public LiveSplitState State { get; set; }
        //public MediaPlayer.IMediaPlayer Player { get; set; }
        public SpeechSettings Settings { get; set; }

        public bool Activated { get; set; }

        public override string ComponentName
        {
            get { return "Speech"; }
        }

        public SpeechComponent(LiveSplitState state)
        {
            Settings = new SpeechSettings();
            State = state;
            //Player = new MediaPlayer.MediaPlayer();
            Activated = true;

            State.OnStart += State_OnStart;
            State.OnSplit += State_OnSplit;
            State.OnSkipSplit += State_OnSkipSplit;
            State.OnUndoSplit += State_OnUndoSplit;
            State.OnPause += State_OnPause;
            State.OnResume += State_OnResume;
            State.OnReset += State_OnReset;
        }

        void State_OnReset(object sender, TimerPhase e)
        {
            if (e != TimerPhase.Ended)
                PlaySound("Timer resetted");
        }

        void State_OnResume(object sender, EventArgs e)
        {
            PlaySound("Timer resumed");
        }

        void State_OnPause(object sender, EventArgs e)
        {
            PlaySound("Timer paused");
        }

        void State_OnUndoSplit(object sender, EventArgs e)
        {
            PlaySound("Split undone. You currently are at " + State.CurrentSplit.Name);
        }

        void State_OnSkipSplit(object sender, EventArgs e)
        {
            PlaySound("Skipped Split. You currently are at " + State.CurrentSplit.Name);
        }

        void State_OnSplit(object sender, EventArgs e)
        {
            if (State.CurrentPhase == TimerPhase.Ended)
            {
                var text = GetSoundTextForLastSplit();
                PlaySound(text);
            }
            else
            {
                var text = GetSoundTextForSplit();
                PlaySound(text);
            }
        }

        public String GetSoundTextForSplit()
        {
            var splitIndex = State.CurrentSplitIndex - 1;
            var timeDifference = State.Run[State.CurrentSplitIndex - 1].SplitTime[State.CurrentTimingMethod] - State.Run[State.CurrentSplitIndex - 1].Comparisons[State.CurrentComparison][State.CurrentTimingMethod];
            String text = null;
            if (timeDifference != null)
            {
                var timeDifferenceText = FormatTime(timeDifference.Value);
                var previousSegment = LiveSplitStateHelper.GetPreviousSegment(State, splitIndex, false, false, true, State.CurrentComparison, State.CurrentTimingMethod);
                var previousSegmentText = FormatTime(previousSegment ?? TimeSpan.Zero);

                if (timeDifference < TimeSpan.Zero)
                {
                    text = "You are " + timeDifferenceText + " ahead at " + State.Run[State.CurrentSplitIndex - 1].Name + ". ";
                    if (previousSegment > TimeSpan.Zero)
                        text += "You lost " + previousSegmentText + ". ";
                    else
                        text += "You gained " + previousSegmentText + ". ";
                }
                else
                {
                    text = "You are " + timeDifferenceText + " behind at " + State.Run[State.CurrentSplitIndex - 1].Name + ". ";
                    if (previousSegment < TimeSpan.Zero)
                        text += "You gained " + previousSegmentText + ". ";
                    else
                        text += "You lost " + previousSegmentText + ". ";
                }
            }
            else
            {
                text = "You did " + State.Run[State.CurrentSplitIndex - 1].Name  + " in " + FormatTime(State.Run[State.CurrentSplitIndex - 1].SplitTime[State.CurrentTimingMethod].Value) + ". ";
            }
            //Check for best segment
            TimeSpan? curSegment;
            curSegment = LiveSplitStateHelper.GetPreviousSegment(State, splitIndex, false, true, true, State.CurrentComparison, State.CurrentTimingMethod);
            if (curSegment != null)
            {
                if (State.Run[splitIndex].BestSegmentTime[State.CurrentTimingMethod] == null || curSegment < State.Run[splitIndex].BestSegmentTime[State.CurrentTimingMethod])
                    text += "The last segment was a best segment.";
            }
            return text;
        }

        public String GetSoundTextForLastSplit()
        {
            var runName = State.Run.GameName + " " + State.Run.CategoryName;
            if (State.Run.GameName.Length + State.Run.CategoryName.Length == 0)
                runName = "the run";

            String text = "You did " + runName + " in " + FormatTime(State.Run.Last().SplitTime[State.CurrentTimingMethod].Value) + ". ";
            var timeDifference = State.Run.Last().SplitTime[State.CurrentTimingMethod] - State.Run.Last().Comparisons[State.CurrentComparison][State.CurrentTimingMethod];

            if (State.Run.Last().PersonalBestSplitTime[State.CurrentTimingMethod] == null
                || State.Run.Last().SplitTime[State.CurrentTimingMethod] < State.Run.Last().PersonalBestSplitTime[State.CurrentTimingMethod])
                text += "You got a new Personal Best. ";
            else
                text += "You unfortunately did not get a new Personal Best. ";

            if (timeDifference != null)
            {
                var timeDifferenceText = FormatTime(timeDifference.Value);
                if (timeDifference < TimeSpan.Zero)
                    text += "You improved your run by " + timeDifferenceText + ". ";
                else
                    text += "You were behind by " + timeDifferenceText + ". ";
            }

            //Check for best segment
            TimeSpan? curSegment;
            var splitIndex = State.CurrentSplitIndex - 1;
            curSegment = LiveSplitStateHelper.GetPreviousSegment(State, splitIndex, false, true, true, State.CurrentComparison, State.CurrentTimingMethod);
            if (curSegment != null)
            {
                if (State.Run[splitIndex].BestSegmentTime[State.CurrentTimingMethod] == null || curSegment < State.Run[splitIndex].BestSegmentTime[State.CurrentTimingMethod])
                    text += "The last segment was a best segment. ";
            }

            return text;
        }

        private String FormatTime(TimeSpan time)
        {
            var builder = new StringBuilder();

            if (time < TimeSpan.Zero)
            {
                time = TimeSpan.Zero - time;
            }

            var count = 0;
            var totalCount = (time.TotalHours >= 1 ? 1 : 0)
                + (time.Minutes >= 1 ? 1 : 0)
                + (time.Seconds >= 1 ? 1 : 0);

            Action insertAndMaybe = () =>
                {
                    if (count != 0 && count == totalCount - 1)
                        builder.Append("and ");
                };

            if (time.TotalHours >= 1)
            {
                builder.Append((int)time.TotalHours);
                builder.Append(" hour");
                if (time.TotalHours >= 2 || time.TotalHours < 1)
                    builder.Append("s");
                builder.Append(" ");
                count++;
            }

            insertAndMaybe();

            if (time.Minutes >= 1)
            {
                builder.Append(time.Minutes);
                builder.Append(" minute");
                if (time.Minutes != 1)
                    builder.Append("s");
                builder.Append(" ");
                count++;
            }

            insertAndMaybe();

            if (time.Seconds >= 1 || count == 0)
            {
                builder.Append(time.Seconds);
                if (count == 0)
                {
                    builder.Append(" point ");
                    builder.Append((int)((time.TotalSeconds % 1) * 10));
                }
                builder.Append(" second");
                if (time.Seconds != 1 || count == 0)
                    builder.Append("s");
                builder.Append(" ");
                count++;
            }

            return builder.ToString();
        }

        void State_OnStart(object sender, EventArgs e)
        {
            PlaySound("Timer started");
        }

        public override Control GetSettingsControl(LayoutMode mode)
        {
            return null;
        }

        public override System.Xml.XmlNode GetSettings(System.Xml.XmlDocument document)
        {
            return document.CreateElement("Settings");
            //return Settings.GetSettings(document);
        }

        public override void SetSettings(System.Xml.XmlNode settings)
        {
            //Settings.SetSettings(settings);
        }

        public override void RenameComparison(string oldName, string newName)
        {
        }

        public override void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {
        }

        public void PlaySound(String text)
        {
            if (Activated && !String.IsNullOrEmpty(text))
            {
                Task.Factory.StartNew(() =>
                {
                    var synth = new SpeechSynthesizer();
                    var voices = synth.GetInstalledVoices();
                    synth.SelectVoiceByHints(VoiceGender.Male, VoiceAge.Adult, 0, new CultureInfo("en-US")); 
                    synth.SpeakAsync(text);
                });
            }
        }

        public override void Dispose()
        {
            State.OnStart -= State_OnStart;
            State.OnSplit -= State_OnSplit;
            State.OnSkipSplit -= State_OnSkipSplit;
            State.OnUndoSplit -= State_OnUndoSplit;
            State.OnPause -= State_OnPause;
            State.OnResume -= State_OnResume;
            State.OnReset -= State_OnReset;
            //Player.Stop();
        }

        
    }
}
