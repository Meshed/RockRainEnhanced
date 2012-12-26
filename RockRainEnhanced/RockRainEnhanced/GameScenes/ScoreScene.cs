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
    using Microsoft.Xna.Framework.Graphics;

    using RockRainEnhanced.ControllerStrategy;
    using RockRainEnhanced.Core;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ScoreScene : GameScene
    {
        private readonly IDictionary<int, Tuple<string, TextComponent, IController>> _highScores =
            new Dictionary<int, Tuple<string, TextComponent, IController>>();

        private readonly IList<Tuple<int, IController, string>> _playerEntries;

        public bool AcceptingInput { get; private set; }

        public ScoreScene(
            Game game,
            SpriteFont font,
            IDictionary<int, string> highScores,
            Texture2D background,
            params Tuple<int, IController>[] playerScores)
            : base(game)
        {
            var backgroundComponent = new ImageComponent(game, background, ImageComponent.DrawMode.Center);

            Components.Add(backgroundComponent);
            
            
            if (highScores.Count < 10 || playerScores.Any(ps => highScores.Keys.Any(k => k < ps.Item1)))
            {
                AcceptingInput = true;
                var highCount = playerScores.Count(ps => highScores.Keys.Any(k => k < ps.Item1));

                var remove = _highScores.Keys.OrderBy(k => k).Take(highCount);
                foreach (var r in remove)
                {
                    _highScores.Remove(r);
                }
                _playerEntries =
                    playerScores.OrderByDescending(ps => ps.Item1)
                                .Take(highCount)
                                .Select(ps => Tuple.Create(ps.Item1, ps.Item2, string.Empty))
                                .ToList();
                foreach (var pe in _playerEntries)
                {
                    highScores.Add(pe.Item1, pe.Item3);
                }
            }
            var y = 100;
            foreach (var hs in highScores.OrderByDescending(x => x.Key))
            {
                var component = new TextComponent(game, font, new Vector2(100, y), Color.White)
                                    {
                                        Visible = true,
                                        Enabled = true,
                                        Text = hs.Value +"   "+ hs.Key
                                    };
                
                var playerScore = playerScores.LastOrDefault(ps => hs.Key == ps.Item1);
                var controller = playerScore != null ? playerScore.Item2 : null;
                Components.Add(component);
                _highScores.Add(hs.Key, Tuple.Create(string.Empty, component, controller));
                y += 100;
            }
            
        }

        public override void Update(GameTime gameTime)
        {
            if (_playerEntries != null && _playerEntries.Any())
            {
                foreach (var pe in _playerEntries.Where(p => p.Item2.IsEnter).ToArray())
                {
                    //_highScores.Add(pe.Item1, pe.Item3);
                    //_playerEntries.Remove(pe);
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
