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

        [Configurable] public string JudgeSprite = "sb/judge.png";
        [Configurable] public string LaneSprite = "sb/lane.jpg";

        [Configurable] public string NoteSprite = "sb/note.png";

        [Configurable] public string HoldNoteSprite = "sb/hold-note.png";

        [Configurable] public string HoldSprite = "sb/hold.png";

        [Configurable] public string OsuDifficulty = "Expert";

        [Configurable] public int LaneYOffset = 430;

        [Configurable] public double LaneScale = 0.3;

        [Configurable] public double HoldNoteScale = 4.8; // for 30 speed, 3.0 for 50 speed (slower)

        [Configurable] public double LaneSrollSpeed = 10;

        private const int endX = 320;

        private const double BeatDuration = 448;


        public override void Generate()
        {

            var Map = GetBeatmap(OsuDifficulty);


            // Log(OsuHitObject.WidescreenStoryboardBounds);
            var lastObject = Map.HitObjects.Last();


            var lane = GetLayer("Lane Left").CreateSprite(LaneSprite, OsbOrigin.Centre);

            lane.ScaleVec(0, 3.25, LaneScale);
            lane.MoveY(0, LaneYOffset);
            lane.MoveX(0, -106);
            lane.Fade(0, lastObject.EndTime, 1, 1);



            var laneRight = GetLayer("Lane Right").CreateSprite(LaneSprite, OsbOrigin.Centre);

            laneRight.ScaleVec(0, 3.25, LaneScale);
            laneRight.MoveY(0, LaneYOffset);
            laneRight.MoveX(0, 746);
            laneRight.Fade(0, lastObject.EndTime, 1, 1);


            MakeNotes();

            var judge = GetLayer("Judge").CreateSprite(JudgeSprite, OsbOrigin.Centre);
            judge.Scale(0, LaneScale);
            judge.Fade(0, lastObject.EndTime, 1, 1);
            judge.MoveY(0, LaneYOffset);


            var lastFrame = 1236;
            var firstFrame = 1012;


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


            var StartTime = EndTime - LaneSrollSpeed * 30;
            note.Scale(0, LaneScale);
            note.Fade(StartTime, StartTime, 0, 1);
            note.Fade(EndTime, EndTime, 1, 0);
            note.Move(StartTime, EndTime, startX, LaneYOffset, endX, LaneYOffset);
        }

        private void renderSliderNote(double StartTime, double EndTime, Vector2 column)
        {
            renderSliderHold(StartTime, EndTime, column);
            renderTapNote(StartTime, column, HoldNoteSprite);
            renderTapNote(EndTime, column, HoldNoteSprite);

        }

        private void renderSliderHold(double StartTime, double EndTime, Vector2 column)
        {
            var scale = getScaleHold(StartTime, EndTime);
            var note = GetLayer(getLaneName(column)).CreateSprite(HoldSprite, OsbOrigin.Centre);

            var startX = -106;
            var dt = (EndTime - StartTime) / 2;
            EndTime = EndTime - dt;

            StartTime = EndTime - LaneSrollSpeed * 30;

            if (!isLeft(column))
            {
                startX = 746;
            }

            /**

                4/4 beat = 448 ms

                1012 -> 1236 = 224 => 2/4 beat     224 / 448 * x = 2.4  , x = 4.8

                12206 -> 12541 = 335 => 3/4 beat = 3.6      335 / 448 * x = 3.6,   x = 4.8

            **/

            note.ScaleVec(0, scale, LaneScale);
            note.Fade(StartTime, StartTime, 0, 1);
            note.Fade(EndTime, EndTime, 1, 0);
            note.Move(StartTime, EndTime, startX, LaneYOffset, endX, LaneYOffset);
        }

        private double getScaleHold(double StartTime, double EndTime)
        {
            var dt = EndTime - StartTime;
            return (dt * HoldNoteScale) / BeatDuration;
        }
    }
}
