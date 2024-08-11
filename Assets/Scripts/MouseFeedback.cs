using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseFeedback : MonoBehaviour
{
    public bool changeMouseOnClick = true;
    public bool neededClicker = false;
    public Texture2D normalCursor;
    public Texture2D clickCursor;
    public Texture2D needClickCursor;

    private float _currentTime = 0;
    private float _clickTime = 0.2f;
    private bool _isNormal = true;

    void Update()
    {
        if(neededClicker)
        {
            _currentTime += Time.deltaTime;
            if(_currentTime >= _clickTime)
            {
                _currentTime = 0;
                _isNormal = !_isNormal;
                Cursor.SetCursor( _isNormal ? normalCursor : needClickCursor, Vector2.zero, CursorMode.Auto);
            }
        }
        if(changeMouseOnClick)
        {
            if(Input.GetMouseButtonDown(0))
            {
                Cursor.SetCursor(clickCursor, Vector2.zero, CursorMode.Auto);
            }
            else if(Input.GetMouseButtonUp(0))
            {
                Cursor.SetCursor(normalCursor, Vector2.zero, CursorMode.Auto);
            }
        }
    }
}
