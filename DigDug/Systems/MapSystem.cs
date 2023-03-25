using System;
using System.Collections.Generic;
using System.Linq;
using DigDug.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DigDug.Systems;

public class MapSystem : System
{
    private enum TileColor
    {
        Yellow,
        Orange,
        Maroon,
        Red,
    }

    public enum HoleDirection
    {
        Horizontal,
        Vertical
    }

    private readonly List<Point> _holes = new();
    private readonly List<LineSegment> _verticalHoles = new();
    private readonly List<LineSegment> _horizontalHoles = new();
    private readonly Dictionary<string, Texture2D> _mapTiles;
    private readonly List<List<TileColor>> _tiles = new();
    private const int TileWidth = 50;
    private const int TileHeight = 50;
    private readonly List<int> _randomOffsets = new();
        
    public MapSystem(Dictionary<string, Texture2D> mapTiles)
    {
        _mapTiles = mapTiles;
        foreach(TileColor color in Enum.GetValues(typeof(TileColor)))
            for (var i = 0; i < 8; i++)
            {
                var list = new List<TileColor>();
                for(var j = 0; j < 30; j++)
                    list.Add(color);
                _tiles.Add(list);
            }
        var rand = new Random();
        for(var i = 0; i < 30; i++)
            _randomOffsets.Add(rand.Next() % 8);
    }

    public bool AddDugOutArea(Point position, HoleDirection direction)
    {
        if (position.Y < Constants.SKY_HEIGHT + Constants.DUG_DIMENSIONS * .6)
            return false;
        return ModifyClosestHoleSegment(position, direction);
    }

    private bool ModifyClosestHoleSegment(Point position, HoleDirection direction)
    {
        var newPosition = new Point(position.X, position.Y);
        var list = _verticalHoles;                
        if (direction == HoleDirection.Horizontal)
        {
            newPosition.X += 10;
            list = _horizontalHoles;
        }
        else
            newPosition.Y += 10;
        if (!list.Any())
        {
            list.Add(new LineSegment(position, newPosition));
            return true;
        }

        var closestStart = list.Min(p => p.DistanceFromStart(position));
        var closestEnd = list.Min(p => p.DistanceFromEnd(position));
        LineSegment closestSegment;
        if (closestStart >= closestEnd)
        {
            closestSegment = list.First(s => Math.Abs(s.DistanceFromEnd(position) - closestEnd) < Constants.TOLERANCE);
            if (closestSegment.IsInMiddle(position, direction))
                return false;
            if(closestEnd < 50)
                closestSegment.ExtendEnd(position);
            else
                list.Add(new LineSegment(position, newPosition));
        }
        else
        {
            closestSegment = list.First(s => Math.Abs(s.DistanceFromStart(position) - closestStart) < Constants.TOLERANCE);
            if (closestSegment.IsInMiddle(position, direction))
                return false;   
            if (closestStart < 50)
                closestSegment.ExtendStart(position);
            else
               list.Add(new LineSegment(position, newPosition));
        }

        return true;
    }
    
    public override void Update(GameTime gameTime) { }

    public override void Render(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin();
        RenderSky(spriteBatch);        
        RenderSkyToDirt(spriteBatch);        
        RenderDirt(spriteBatch);

        const int holeDimension = Constants.DUG_DIMENSIONS + Constants.HOLE_BUFFER * 2;
        RenderHorizontalHoles(spriteBatch, holeDimension);
        RenderVerticalHoles(spriteBatch, holeDimension);
        
        spriteBatch.End();
    }

    private void RenderSky(SpriteBatch spriteBatch)
    {
        var tilesForWidth = Math.Ceiling((double)Constants.GAME_WIDTH / TileWidth);
        var tilesForHeight = Math.Ceiling((double)Constants.GAME_HEIGHT / TileHeight);
        for(var i = 0; i < tilesForHeight; i++)
            for(var j = 0; j < tilesForWidth; j++)
                spriteBatch.Draw(_mapTiles["Sky"], 
                    new Rectangle(j * TileWidth, i * TileHeight, TileWidth, TileHeight),
                    new Rectangle(0, 0, 60, 60),
                    Color.White,
                    0,
                    new Vector2(),
                    SpriteEffects.None,
                    0);
    }

    private void RenderSkyToDirt(SpriteBatch spriteBatch)
    {
        var tilesForWidth = Math.Ceiling((double)Constants.GAME_WIDTH / TileWidth);
        for(var j = 0; j < tilesForWidth; j++)
            spriteBatch.Draw(_mapTiles["SkyToDirt"], 
                new Rectangle(j * TileWidth,  Constants.SKY_HEIGHT, TileWidth * 2, TileHeight * 2),
                new Rectangle(0, 0, 60, 60),
                Color.White,
                0,
                new Vector2(),
                SpriteEffects.None,
                0);
    }

    private void RenderDirt(SpriteBatch spriteBatch)
    {
        for(var i = 1; i < _tiles.Count; i++)
            for (var j = 0; j < _tiles[i].Count; j++)
            {
                var offset = 0;
                if (i > 0 && _tiles[i - 1][j] != _tiles[i][j])
                    offset = _randomOffsets[j];
                spriteBatch.Draw(_mapTiles[_tiles[i][j].ToString()],
                    new Rectangle(j * TileWidth, i * TileHeight + offset + Constants.SKY_HEIGHT, TileWidth + 13, TileHeight + 13),
                    new Rectangle(0, 0, 90, 90),
                    Color.White,
                    0,
                    new Vector2(),
                    SpriteEffects.None,
                    0);
            }
    }
    
    private void RenderVerticalHoles(SpriteBatch spriteBatch, int holeDimension)
    {
        foreach (var vertLine in _verticalHoles)
        {
            if(Utilities.Utilities.CalculateDistance(vertLine.StartPoint, vertLine.MidPoint) > 25)
                spriteBatch.Draw(_mapTiles["Hole"],
                    new Rectangle(vertLine.StartPoint.X, vertLine.StartPoint.Y, holeDimension, holeDimension),
                    new Rectangle(0, 0, 160, 160),
                    Color.White,
                    (float)Constants.PI,
                    new Vector2(80),
                    SpriteEffects.None,
                    0);
            var renderingPoint = vertLine.MidPoint;
            var startPoint = vertLine.StartPoint;
            startPoint.Y += Constants.DUG_DIMENSIONS;
            while (Utilities.Utilities.CalculateDistance(renderingPoint, startPoint) > 6400)
            {
                spriteBatch.Draw(_mapTiles["Hole"],
                    new Rectangle(renderingPoint.X, renderingPoint.Y - 100, holeDimension, holeDimension),
                    new Rectangle(0, 0, 160, 160),
                    Color.White,
                    (float)Constants.PI,
                    new Vector2(80),
                    SpriteEffects.None,
                    0);
                renderingPoint.Y -= 60;
            }

            renderingPoint = vertLine.MidPoint;
            var endPoint = vertLine.EndPoint;
            endPoint.Y += Constants.DUG_DIMENSIONS;
            while (Utilities.Utilities.CalculateDistance(renderingPoint, endPoint) > 6400)
            {
                spriteBatch.Draw(_mapTiles["Hole"],
                    new Rectangle(renderingPoint.X, renderingPoint.Y, holeDimension, holeDimension),
                    new Rectangle(0, 0, 160, 160),
                    Color.White,
                    0,
                    new Vector2(80),
                    SpriteEffects.None,
                    0);
                renderingPoint.Y += 60;
            }
            if(Utilities.Utilities.CalculateDistance(vertLine.EndPoint, vertLine.MidPoint) > 25)
                spriteBatch.Draw(_mapTiles["Hole"],
                    new Rectangle(vertLine.EndPoint.X, vertLine.EndPoint.Y, holeDimension, holeDimension),
                    new Rectangle(0, 0, 160, 160),
                    Color.White,
                    0,
                    new Vector2(80),
                    SpriteEffects.None,
                    0);
        }
    }
    
    
    private void RenderHorizontalHoles(SpriteBatch spriteBatch, int holeDimension)
    {
        foreach (var horizLine in _horizontalHoles)
        {
            if(Utilities.Utilities.CalculateDistance(horizLine.StartPoint, horizLine.MidPoint) > 25)
                spriteBatch.Draw(_mapTiles["Hole"],
                    new Rectangle(horizLine.StartPoint.X, horizLine.StartPoint.Y, holeDimension, holeDimension),
                    new Rectangle(0, 0, 160, 160),
                    Color.White,
                    (float) Constants.PI_OVER_TWO,
                    new Vector2(80),
                    SpriteEffects.None,
                    0);
            var renderingPoint = horizLine.MidPoint;
            var startPoint = horizLine.StartPoint;
            startPoint.X -= Constants.DUG_DIMENSIONS;
            while (Utilities.Utilities.CalculateDistance(renderingPoint, startPoint) > 6400)
            {
                spriteBatch.Draw(_mapTiles["Hole"],
                    new Rectangle(renderingPoint.X, renderingPoint.Y, holeDimension, holeDimension),
                    new Rectangle(0, 0, 160, 160),
                    Color.White,
                    (float) Constants.PI_OVER_TWO,
                    new Vector2(80),
                    SpriteEffects.None,
                    0);
                renderingPoint.X -= 85;
            }
            renderingPoint = horizLine.MidPoint;
            var endPoint = horizLine.EndPoint;
            endPoint.X += Constants.DUG_DIMENSIONS;
            while (Utilities.Utilities.CalculateDistance(renderingPoint, endPoint) > 6400)
            {
                spriteBatch.Draw(_mapTiles["Hole"],
                    new Rectangle(renderingPoint.X, renderingPoint.Y, holeDimension, holeDimension),
                    new Rectangle(0, 0, 160, 160),
                    Color.White,
                    (float)-Constants.PI_OVER_TWO,
                    new Vector2(80),
                    SpriteEffects.None,
                    0);
                renderingPoint.X += 85;
            }
            if(Utilities.Utilities.CalculateDistance(horizLine.EndPoint, horizLine.MidPoint) > 25)
                spriteBatch.Draw(_mapTiles["Hole"],
                    new Rectangle(horizLine.EndPoint.X, horizLine.EndPoint.Y, holeDimension, holeDimension),
                    new Rectangle(0, 0, 160, 160),
                    Color.White,
                    (float)-Constants.PI_OVER_TWO,
                    new Vector2(80),
                    SpriteEffects.None,
                    0);
        }
    }
}