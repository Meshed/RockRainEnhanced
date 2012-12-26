// -----------------------------------------------------------------------
// <copyright file="ScoreScene.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace RockRainEnhanced.GameScenes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Microsoft.Xna.Framework;

    using RockRainEnhanced.ControllerStrategy;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ScoreScene : GameScene
    {
        private readonly IDictionary<int, string> _highScores;

        private readonly IList<Tuple<int, IController, string>> _playerEntries;

        public bool AcceptingInput { get; private set; }
        public ScoreScene(Game game, IDictionary<int, string> highScores, params Tuple<int, IController>[] playerScores)
            : base(game)
        {
            _highScores = highScores;
            if (highScores.Count < 10 || playerScores.Any(ps => highScores.Keys.Any(k => k < ps.Item1)))
            {
                AcceptingInput = true;
                var highCount = playerScores.Count(ps => highScores.Keys.Any(k => k < ps.Item1));

                var remove = _highScores.Keys.OrderBy(k => k).Take(highCount);
                foreach (var r in remove)
                {
                    _highScores.Remove(r);
                }
                _playerEntries = playerScores.OrderByDescending(ps => ps.Item1).Take(highCount).Select(ps=>Tuple.Create(ps.Item1,ps.Item2,string.Empty)).ToList();
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (_playerEntries != null && _playerEntries.Any())
            {
                foreach (var pe in _playerEntries.Where(p => p.Item2.IsEnter).ToArray())
                {
                    _highScores.Add(pe.Item1, pe.Item3);
                    _playerEntries.Remove(pe);
                }
            }
            base.Update(gameTime);

        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
