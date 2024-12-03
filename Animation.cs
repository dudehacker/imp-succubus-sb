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
    public class Animation : StoryboardObjectGenerator
    {
        [Configurable] public bool enabled = false;

        [Configurable] public double tapDurationBeat = 0.5;

        [Configurable] public double BeatDurationMs = 448;

        [Configurable] public string SpritePath = "patch/motion.jpg";

        [Configurable] public string SpritePathAlt = "patch/back.jpg";

        [Configurable] public int FrameCount = 7;

        [Configurable] public int StopIndex = 2;

        [Configurable] public int FrameCountBack = 18;

        [Configurable] public int BackStopIndex = 5;

        [Configurable] public string OsuDifficulty = "Hard";

        private static string bgSprite = "patch/motion0.jpg";

        private static string bgSpriteBack = "patch/back0.jpg";

        public override void Generate()
        {
            if (enabled)
            {
                main();
            }

        }

        private void createBG()
        {

            var Map = GetBeatmap(OsuDifficulty);


            foreach (var it in Map.ControlPoints.Select((x, i) => new { Value = x, Index = i }))
            {

                ControlPoint next = null;

                if (it.Index + 1 < Map.ControlPoints.Count())
                {
                    next = Map.ControlPoints.ElementAt(it.Index + 1);
                }

                double endTime = Beatmap.HitObjects.LastOrDefault()?.EndTime ?? AudioDuration;
                if (next != null)
                {
                    endTime = next.Offset;
                }

                if (it.Value.IsKiai)
                {
                    var bitmap = GetMapsetBitmap(bgSpriteBack);
                    var bg = GetLayer("BG").CreateSprite(bgSpriteBack, OsbOrigin.Centre);
                    bg.Scale(it.Value.Offset, 480.0f / bitmap.Height);
                    bg.Fade(it.Value.Offset, endTime, 1, 1);
                }
                else
                {
                    var bitmap = GetMapsetBitmap(bgSprite);
                    var bg = GetLayer("BG").CreateSprite(bgSprite, OsbOrigin.Centre);
                    bg.Scale(it.Value.Offset, 480.0f / bitmap.Height);
                    bg.Fade(it.Value.Offset, endTime, 1, 1);
                }

            }


        }

        private void main()
        {

            var Map = GetBeatmap(OsuDifficulty);


            createBG();
            foreach (var it in Map.HitObjects.Select((x, i) => new { Value = x, Index = i }))
            {
                ControlPoint timing = Map.GetControlPointAt((int)it.Value.StartTime);
                OsuHitObject next = null;
                if (it.Index + 1 < Map.HitObjects.Count())
                {
                    next = Map.HitObjects.ElementAt(it.Index + 1);
                }

                double nextTime = Beatmap.HitObjects.LastOrDefault()?.EndTime ?? AudioDuration;
                if (next != null)
                {
                    nextTime = next.StartTime;
                }

                animate(it.Value.StartTime, it.Value.EndTime, nextTime, timing.IsKiai);
            }

            // animateTap(2000, 400, false);
        }

        private void animate(double startTime, double endTime, double nextNoteStart, bool isKiai)
        {

            if (startTime == endTime)
            {
                animateTap(startTime, nextNoteStart, isKiai);
            }
            else
            {
                animateHold(startTime, endTime, nextNoteStart, isKiai);
            }

        }

        private void animateTap(double startTime, double nextNoteStart, bool isKiai)
        {

            if (!isKiai)
            {
                OsbAnimation animation = GetLayer("Animation").CreateAnimation(SpritePath, FrameCount, BeatDurationMs / FrameCount, OsbLoopType.LoopOnce, OsbOrigin.Centre);
                animation.Fade(startTime, startTime + animation.FrameCount * animation.FrameDelay, 1, 1);
                var bitmap = GetMapsetBitmap(bgSprite);
                animation.Scale(startTime, 480.0f / bitmap.Height);
            }
            else
            {
                OsbAnimation animation = GetLayer("Animation").CreateAnimation(SpritePathAlt, FrameCountBack, BeatDurationMs / FrameCountBack, OsbLoopType.LoopOnce, OsbOrigin.Centre);
                animation.Fade(startTime, startTime + animation.FrameCount * animation.FrameDelay, 1, 1);
                var bitmap = GetMapsetBitmap(bgSpriteBack);
                animation.Scale(startTime, 480.0f / bitmap.Height);
            }

        }

        private void animateHold(double startTime, double endTime, double nextNoteStart, bool isKiai)
        {
            // to fix the hold to stop in middle animation
            animateTap(startTime, nextNoteStart, isKiai);
        }
    }
}
