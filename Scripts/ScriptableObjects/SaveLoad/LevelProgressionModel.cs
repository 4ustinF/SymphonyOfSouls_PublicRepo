public class LevelProgressionModel 
{
    private int _level = 0;
    private float _points = 0.0f;
    private float _time = 0.0f;
    private float _score=0.0f;

    public LevelProgressionModel(int level, float points, float time)
    {
        _level = level;
        _points = points;
        _time = time;
        _score = GetScore();
    }

    public float GetScore()
    {
        //score = leftover time * total points collected
        return _time * _points; 
    }
}
