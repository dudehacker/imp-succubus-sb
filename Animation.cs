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

        [Configurable] public string SpritePath = "motion.jpg";

        [Configurable] public int FrameCount = 7;
        [Configurable] public string OsuDifficulty = "Hard";

        private static string bgSprite = "patch/motion0.jpg";

        public override void Generate()
        {
            if (enabled)
            {
                main();
            }

        }

        private void main()
        {

            var Map = GetBeatmap(OsuDifficulty);


            foreach (var hitobject in Map.HitObjects)
            {
                animate(hitobject.StartTime, hitobject.EndTime, 2, false);
            }

            // animateTap(2000, 400, false);
        }

        private void animate(double startTime, double endTime, double nextNoteStart, bool isKiai)
        {

            animateTap(startTime, nextNoteStart, isKiai);

        }

        private void animateTap(double startTime, double nextNoteStart, bool isKiai)
        {



            OsbAnimation animation = GetLayer("Animation").CreateAnimation(SpritePath, FrameCount, BeatDurationMs / FrameCount, OsbLoopType.LoopOnce, OsbOrigin.Centre);
            animation.Fade(startTime, startTime + animation.FrameCount * animation.FrameDelay, 1, 1);
            var bitmap = GetMapsetBitmap(bgSprite);
            animation.Scale(startTime, 480.0f / bitmap.Height);
        }

        private void animateHold(double startTime, double endTime, double nextNoteStart)
        {

        }
    }
}
