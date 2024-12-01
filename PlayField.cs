using OpenTK;
using OpenTK.Graphics;
using StorybrewCommon.Mapset;
using StorybrewCommon.Scripting;
using StorybrewCommon.Storyboarding;
using StorybrewCommon.Storyboarding.Util;
using StorybrewCommon.Subtitles;
using StorybrewCommon.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StorybrewScripts
{
    public class PlayField : StoryboardObjectGenerator
    {

        [Group("Sprite")]
        [Configurable] public string JudgeSprite = "sb/judge.png";
        [Configurable] public string LaneSprite = "sb/lane.jpg";

        [Configurable] public string NoteSprite = "sb/note.png";

        [Configurable] public string HoldNoteSprite = "sb/hold-note.png";

        [Configurable] public string HoldSprite = "sb/hold.png";

        [Group("Config")]

        [Description("Osu Difficulty name to create the SB from")]
        [Configurable] public string OsuDifficulty = "Expert";

        [Description("Reduce number to move lane up")]
        [Configurable] public int LaneYOffset = 430;

        [Description("Increase number to make lane bigger")]
        [Configurable] public double LaneScale = 0.3;

        [Description("Change Note Speed, bigger number = slower")]
        [Configurable] public double LaneSrollSpeed = 30;

        private const int endX = 320;

        private const double BeatDuration = 448;


        public override void Generate()
        {

            var Map = GetBeatmap(OsuDifficulty);


            // Log(OsuHitObject.WidescreenStoryboardBounds);
            var lastObject = Map.HitObjects.Last();


            var lane = GetLayer("Lane").CreateSprite(LaneSprite, OsbOrigin.Centre);

            lane.ScaleVec(0, 4, LaneScale);
            lane.MoveY(0, LaneYOffset);
            lane.Fade(0, lastObject.EndTime, 1, 1);


            MakeNotes();

            var judge = GetLayer("Judge").CreateSprite(JudgeSprite, OsbOrigin.Centre);
            judge.Scale(0, LaneScale);
            judge.Fade(0, lastObject.EndTime, 1, 1);
            judge.MoveY(0, LaneYOffset);

            // renderSliderNote(1012, 1236, new Vector2(128, 192));
            // renderSliderNote(12206, 12541, new Vector2(128, 192));


        }


        private void MakeNotes()
        {
            var Map = GetBeatmap(OsuDifficulty);
            foreach (var hitobject in Map.HitObjects)
            {
                MakeNote(hitobject.StartTime, hitobject.EndTime, hitobject.PlayfieldPosition);
            }
        }

        private bool isLeft(Vector2 position)
        {
            return Math.Floor(position.X * 2 / 512) == 0;
        }

        private string getLaneName(Vector2 position)
        {
            if (isLeft(position))
            {
                return "Left Lane Notes";
            }
            return "Right Lane Notes";
        }

        private void MakeNote(double StartTime, double EndTime, Vector2 column)
        {

            var isSlider = EndTime != StartTime;
            if (isSlider)
            {
                renderSliderNote(StartTime, EndTime, column);
            }
            else
            {
                renderTapNote(StartTime, column);
            }

        }

        private void renderTapNote(double EndTime, Vector2 column)
        {
            renderTapNote(EndTime, column, NoteSprite);
        }

        private void renderTapNote(double EndTime, Vector2 column, string sprite)
        {

            var note = GetLayer(getLaneName(column)).CreateSprite(sprite, OsbOrigin.Centre);

            var startX = -106;
            if (!isLeft(column))
            {
                startX = 746;
            }


            var StartTime = getStartTime(EndTime);
            note.Scale(0, LaneScale);
            note.Fade(StartTime, StartTime, 0, 1);
            note.Fade(EndTime, EndTime, 1, 0);
            note.Move(StartTime, EndTime, startX, LaneYOffset, endX, LaneYOffset);
        }

        private double getStartTime(double EndTime)
        {
            return EndTime - LaneSrollSpeed * 30;
        }

        private void renderSliderNote(double StartTime, double EndTime, Vector2 column)
        {
            renderSliderHold(StartTime, EndTime, column);
            renderTapNote(StartTime, column, HoldNoteSprite);
            renderTapNote(EndTime, column, HoldNoteSprite);

        }

        private OsbOrigin getOrigin(Vector2 column)
        {
            if (isLeft(column))
            {
                return OsbOrigin.CentreRight;
            }
            return OsbOrigin.CentreLeft;
        }

        private void renderSliderHold(double StartTime, double EndTime, Vector2 column)
        {
            var scale = getScaleHold(StartTime, EndTime);
            var note = GetLayer(getLaneName(column)).CreateSprite(HoldSprite, getOrigin(column));

            var BeforeStartTimeEarlier = getStartTime(StartTime);
            var startX = -106;

            if (!isLeft(column))
            {
                startX = 746;
            }

            note.ScaleVec(BeforeStartTimeEarlier, StartTime, scale, LaneScale, scale, LaneScale);
            note.ScaleVec(StartTime, EndTime, scale, LaneScale, 0, LaneScale);

            note.Fade(BeforeStartTimeEarlier, 1);

            note.Move(BeforeStartTimeEarlier, StartTime, startX, LaneYOffset, endX, LaneYOffset);
        }

        private double getScaleHold(double StartTime, double EndTime)
        {
            var dt = EndTime - StartTime;
            return dt * (150 / LaneSrollSpeed) / BeatDuration;
        }
    }
}
