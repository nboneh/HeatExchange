using UnityEngine;
using System.Collections;

public class DriverScript : MonoBehaviour {

    enum GameState {Logo, Intro, MainMenu, Credits, Instructions, Play, Pause, GameOver, Win, BlackScreen, NoState };
    float alpha = 0.0f;
    public Font font;
    public Texture introImage;
    public Camera objCamera;
    public MainCharacter mainCharacter;
    public AudioClip clickSound;

    private AudioSource source;


    GameState currentState;
    GameState prevState;

    float logoCounter = 0;
    float logoEndTime = 2;

    // Use this for initialization
    void Start () {
        currentState = GameState.Logo;
        prevState = GameState.BlackScreen;

        source = GetComponent<AudioSource>();

        PauseGame();
    }

    void resetGame()
    {
        mainCharacter.reset();
    }
    void PauseGame()
    {
        Time.timeScale = 0.0F;
    }

    void ResumeGame()
    {
        Time.timeScale = 1.0F;
    }

    void SetState(GameState state)
    {
        if (state == GameState.Pause && currentState != GameState.Play)
            return;

        if(state == GameState.Play)
        {
            Cursor.lockState = CursorLockMode.Locked;
            ResumeGame();
        } else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            PauseGame();
        }

        prevState = currentState;
        currentState = state;
        alpha = 0;
    }

    // Update is called once per frame
    void Update () {
        float t = Time.unscaledDeltaTime;
        if (t > .3f)
            return;
        if (alpha < 1.0f)
        {
            alpha += t *2;
            if (alpha > 1.0f)
            {
                alpha = 1.0f;
            }
        }

        if(currentState == GameState.Logo)
        {
            logoCounter += t;
            if(logoCounter >= logoEndTime)
            {
                logoCounter = 0;
                SetState(GameState.Intro);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetState(GameState.Pause);
        }
    }

    void SetAlpha()
    {
        Color c = GUI.color;
        c.a = alpha;
        GUI.color = c;
    }

    void SetPrevAlpha()
    {
        Color c = GUI.color;
        c.a = 1 - alpha;
        GUI.color = c;
    }

    void OnGUI()
    {
        if(isBlackScreenState(prevState) && isBlackScreenState(currentState))
        {
            DrawBlackBackGround();
        }

        if(prevState != GameState.NoState && prevState != GameState.BlackScreen)
        {
            SetPrevAlpha();
            DrawState(prevState);
            if (alpha == 1.0f)
            {
                if (isBlackScreenState(prevState))
                    prevState = GameState.BlackScreen;
                else
                    prevState = GameState.NoState;
                alpha = 0.0f;

                if (currentState == GameState.MainMenu)
                    resetGame();
            }
        } else{
            SetAlpha();
            DrawState(currentState);
        }
    }


    void DrawState(GameState state)
    {
        switch (state)
        {
            case GameState.Logo:
                DrawLogo();
                break;
            case GameState.Intro:
                DrawIntro();
                break;
            case GameState.MainMenu:
                DrawMainMenu();
                break;
            case GameState.Instructions:
                DrawInstructions();
                break;
            case GameState.Credits:
                DrawCredits();
                break;
            case GameState.GameOver:
            case GameState.Win:
            case GameState.Pause:
                DrawBoxMenu(state);
                break;
        }
    }

    bool isBlackScreenState(GameState state)
    {
        return (state == GameState.BlackScreen || state == GameState.Logo || state == GameState.Intro || state ==GameState.MainMenu || state == GameState.Instructions || state == GameState.Credits);
    }
    void DrawMainMenu()
    {
        DrawBlackBackGround();

        GUIStyle textStyle = GUI.skin.GetStyle("Label");
        textStyle.alignment = TextAnchor.MiddleCenter;
        textStyle.normal.textColor = Color.white;
        int fontSize = GetFontSize(70);
        textStyle.fontSize = fontSize;
        textStyle.font = font;


         objCamera.Render();

        float width = fontSize * 36;
        float height = fontSize * 2;
        textStyle.fontStyle = FontStyle.Bold;
        DrawOutline(new Rect(Screen.width / 2 - width / 2, height / 8.0f , width, height), "Heat Exchange", textStyle);

        fontSize = GetFontSize(30);

        GUIStyle buttonStyle = GUI.skin.GetStyle("Button");
        buttonStyle.fontSize = fontSize;
        buttonStyle.font = font;
        float buttonWidth = fontSize * 3.7f;
        float buttonHeight = fontSize * 1.5f;
        if (GUI.Button(new Rect(Screen.width / 2 - buttonWidth / 2, Screen.height - buttonHeight * 5, buttonWidth, buttonHeight), "Play", buttonStyle))
        {
            SetState(GameState.Play);
            source.PlayOneShot(clickSound);
        }
        buttonWidth = fontSize * 9;
        if (GUI.Button(new Rect(Screen.width / 2 - buttonWidth / 2, Screen.height - buttonHeight * 3.3f, buttonWidth, buttonHeight), "Instructions", buttonStyle))
        {
            SetState(GameState.Instructions);
            source.PlayOneShot(clickSound);
        }
        buttonWidth = fontSize * 5.5f;
        if (GUI.Button(new Rect(Screen.width / 2 - buttonWidth / 2, Screen.height - buttonHeight * 1.6f, buttonWidth, buttonHeight), "Credits", buttonStyle))
        {
            SetState(GameState.Credits);
            source.PlayOneShot(clickSound);
        }
    }

    void DrawInstructions()
    {
        DrawBlackBackGround();

        GUIStyle textStyle = GUI.skin.GetStyle("Label");
        textStyle.alignment = TextAnchor.MiddleCenter;
        textStyle.normal.textColor = Color.white;
        int fontSize = GetFontSize(50);
        textStyle.fontSize = fontSize;
        textStyle.font = font;

        float width = fontSize * 36;
        float height = fontSize * 2;
        textStyle.fontStyle = FontStyle.Bold;
        DrawOutline(new Rect(Screen.width / 2 - width / 2, height / 8.0f, width, height), "Instructions", textStyle);

        textStyle = GUI.skin.GetStyle("Label");
        textStyle.alignment = TextAnchor.MiddleCenter;
        textStyle.normal.textColor = Color.white;
        fontSize = GetFontSize(22);
        textStyle.fontSize = fontSize;
        textStyle.font = font;

        width = fontSize * 31;
        height = fontSize * 4.5f;
        textStyle.fontStyle = FontStyle.Bold;
        DrawOutline(new Rect(Screen.width / 2 - width / 2, height * 1f, width, height), "Gather the escaped animals and put them back in the correct pens and remember to fuel up at the charging station.", textStyle);


        height = fontSize * 1.5f;

        DrawOutline(new Rect(Screen.width / 2 - width / 2, height * 6.5f, width, height), "W = Move Forward", textStyle);
        DrawOutline(new Rect(Screen.width / 2 - width / 2, height * 7.5f, width, height), "A = Move Left", textStyle);
        DrawOutline(new Rect(Screen.width / 2 - width / 2, height * 8.5f, width, height), "S = Move Back", textStyle);
        DrawOutline(new Rect(Screen.width / 2 - width / 2, height * 9.5f, width, height), "D = Move Right", textStyle);

        DrawOutline(new Rect(Screen.width / 2 - width / 2, height * 10.5f, width, height), "Tab = Toggle Thrust/Hover Mode", textStyle);
        DrawOutline(new Rect(Screen.width / 2 - width / 2, height * 12.5f, width, height*2), "Space = Jump / Jump onto charging station (Thrust Mode)", textStyle);

        height = fontSize * 1.5f;
        DrawOutline(new Rect(Screen.width / 2 - width / 2, height * 15.5f, width, height), "Left click = Pick up/Set down (Hover Mode)", textStyle);
        DrawOutline(new Rect(Screen.width / 2 - width / 2, height * 16.5f, width, height), "Right click = Toss (Hover Mode)", textStyle);


        DrawOutline(new Rect(Screen.width / 2 - width / 2, height * 18.5f, width, height), "ESC = Pause", textStyle);

        fontSize = GetFontSize(30);
        GUIStyle buttonStyle = GUI.skin.GetStyle("Button");
        buttonStyle.fontSize = fontSize;
        buttonStyle.font = font;
        float buttonWidth = fontSize * 3.7f;
        float buttonHeight = fontSize * 1.5f;
        if (GUI.Button(new Rect(buttonWidth / 2, Screen.height - buttonHeight * 1.2f, buttonWidth, buttonHeight), "Back", buttonStyle))
        {
            SetState(GameState.MainMenu);
            source.PlayOneShot(clickSound);
        }
    }

    void DrawCredits()
    {
        DrawBlackBackGround();

        GUIStyle textStyle = GUI.skin.GetStyle("Label");
        textStyle.alignment = TextAnchor.MiddleCenter;
        textStyle.normal.textColor = Color.white;
        int fontSize = GetFontSize(50);
        textStyle.fontSize = fontSize;
        textStyle.font = font;

        float width = fontSize * 36;
        float height = fontSize * 2;
        textStyle.fontStyle = FontStyle.Bold;
        DrawOutline(new Rect(Screen.width / 2 - width / 2, height / 8.0f, width, height), "Credits", textStyle);

        textStyle = GUI.skin.GetStyle("Label");
        textStyle.normal.textColor = Color.white;
        textStyle.alignment = TextAnchor.MiddleCenter;
        fontSize = GetFontSize(30);
        textStyle.fontSize = fontSize;
        textStyle.font = font;

        width = fontSize * 15;
        height = fontSize * 2;
        float relativeHeight = Screen.height / 15;
        textStyle.fontStyle = FontStyle.Bold;

        textStyle.alignment = TextAnchor.MiddleLeft;
        DrawOutline(new Rect(Screen.width / 2.7f - width / 2, Screen.height / 2 - relativeHeight * (6 - 2 * 1.1f), width, height),"Nir Boneh", textStyle);
        textStyle.alignment = TextAnchor.MiddleRight;
        DrawOutline(new Rect(Screen.width * .62f - width / 2, Screen.height / 2 - relativeHeight * (6 - 2 * 1.1f), width, height), "Code", textStyle);

        textStyle.alignment = TextAnchor.MiddleLeft;
        DrawOutline(new Rect(Screen.width / 2.7f - width / 2, Screen.height / 2 - relativeHeight * (6 - 4 * 1.1f), width, height), "Tyler Barker", textStyle);
        textStyle.alignment = TextAnchor.MiddleRight;
        DrawOutline(new Rect(Screen.width * .62f - width / 2, Screen.height / 2 - relativeHeight * (6 - 4 * 1.1f), width, height), "Graphics", textStyle);

        textStyle.alignment = TextAnchor.MiddleLeft;
        DrawOutline(new Rect(Screen.width / 2.7f - width / 2, Screen.height / 2 - relativeHeight * (6 - 6 * 1.1f), width, height), "Clint Nieman", textStyle);
        textStyle.alignment = TextAnchor.MiddleRight;
        DrawOutline(new Rect(Screen.width * .62f - width / 2, Screen.height / 2 - relativeHeight * (6 - 6 * 1.1f), width, height), "Sound", textStyle);

        fontSize = GetFontSize(30);
        GUIStyle buttonStyle = GUI.skin.GetStyle("Button");
        buttonStyle.fontSize = fontSize;
        buttonStyle.font = font;
        float buttonWidth = fontSize * 3.7f;
        float buttonHeight = fontSize * 1.5f;
        if (GUI.Button(new Rect(buttonWidth / 2, Screen.height - buttonHeight * 1.2f, buttonWidth, buttonHeight), "Back", buttonStyle))
        {
            SetState(GameState.MainMenu);
            source.PlayOneShot(clickSound);
        }
    }

    void DrawLogo()
    {
        DrawBlackBackGround();
        GUIStyle textStyle = GUI.skin.GetStyle("Label");
        textStyle.alignment = TextAnchor.MiddleCenter;
        textStyle.normal.textColor = Color.white;
        int fontSize = GetFontSize(150);
        textStyle.fontSize = fontSize;
        textStyle.font = font;

        float width = fontSize * 36;
        float height = fontSize * 2;
        float relativeHeight = Screen.height / 15;
        textStyle.fontStyle = FontStyle.Bold;
        DrawOutline(new Rect(Screen.width / 2 - width / 2, Screen.height / 2 - relativeHeight * 4f, width, height), "Clouby", textStyle);

        textStyle = GUI.skin.GetStyle("Label");
        textStyle.alignment = TextAnchor.MiddleCenter;
        textStyle.normal.textColor = Color.white;
         fontSize = GetFontSize(50);
        textStyle.fontSize = fontSize;
        textStyle.font = font;

         width = fontSize * 36;
         height = fontSize * 2;
         relativeHeight = Screen.height / 15;
        textStyle.fontStyle = FontStyle.Bold;


        DrawOutline(new Rect(Screen.width / 2 - width / 2, Screen.height / 2 + relativeHeight *1.5f, width, height), "Game Jam 2016", textStyle);
    }

    void DrawBoxMenu(GameState state)
    {
        float boxWidth = Screen.width/1.5f;
        float boxHeight = Screen.height/1.5f;

        float startX = Screen.width / 2 - boxWidth / 2;
        float startY = Screen.height / 2 - boxHeight / 2;



        GUI.Box(new Rect(startX, startY, boxWidth, boxHeight), GUIContent.none);

        GUIStyle textStyle = GUI.skin.GetStyle("Label");
        textStyle.alignment = TextAnchor.MiddleCenter;
        textStyle.normal.textColor = Color.white;
        int fontSize = GetFontSize(50);
        textStyle.fontSize = fontSize;
        textStyle.font = font;

        float width = fontSize * 12;
        float height = fontSize * 2;

        if (state == GameState.Pause)
        {
            DrawOutline(new Rect(Screen.width/2 - width/2, startY+ height/16, width , height) , "Pause", textStyle);
        } else if (state == GameState.GameOver)
        {
            textStyle.normal.textColor = Color.red;
            DrawOutline(new Rect(Screen.width / 2 - width / 2, startY + height / 16, width, height), "GameOver!", textStyle);
        }
        else if (state == GameState.Win)
        {
            textStyle.normal.textColor = Color.green;
            DrawOutline(new Rect(Screen.width / 2 - width / 2, startY + height / 16, width, height), "You Win!", textStyle);
        }


        fontSize = GetFontSize(30);
        GUIStyle buttonStyle = GUI.skin.GetStyle("Button");
        buttonStyle.fontSize = fontSize;
        buttonStyle.font = font;
        float buttonWidth = fontSize * 4.7f;
        float buttonHeight = fontSize * 1.5f;


        if (state == GameState.Pause)
        {
            if (GUI.Button(new Rect(Screen.width / 2 - buttonWidth / 2, Screen.height - buttonHeight * 7, buttonWidth, buttonHeight), "Resume", buttonStyle))
            {
                SetState(GameState.Play);
                source.PlayOneShot(clickSound);
            }
        }
        buttonWidth = fontSize * 6.5f;
        if (GUI.Button(new Rect(Screen.width / 2 - buttonWidth / 2, Screen.height - buttonHeight * 5.5f, buttonWidth, buttonHeight), "New Game", buttonStyle))
        {
            source.PlayOneShot(clickSound);
            SetState(GameState.Play);
            resetGame();
        }
        buttonWidth = fontSize * 7f;
        if (GUI.Button(new Rect(Screen.width / 2 - buttonWidth / 2, Screen.height - buttonHeight * 4f, buttonWidth, buttonHeight), "Main Menu", buttonStyle))
        {
            source.PlayOneShot(clickSound);
            SetState(GameState.MainMenu);
        }

    }

    void DrawIntro()
    {
        DrawBlackBackGround();

        int screenWidth = Screen.width;
        int screenHeight = Screen.height;

        float gap = screenWidth / 30.0f;

        GUI.DrawTexture(new Rect(gap, screenHeight/20.0f, screenWidth - gap*2, screenHeight/2.0f), introImage, ScaleMode.StretchToFill, true, 10.0F);
         GUIStyle textStyle = GUI.skin.GetStyle("Label");
         textStyle.alignment = TextAnchor.UpperLeft;
         textStyle.normal.textColor = Color.white;
         int fontSize = GetFontSize(17);
         textStyle.fontSize = fontSize;
         textStyle.font = font;

         float width = screenWidth/ 1.1f;
         float height = screenHeight/2.2f;
         float relativeHeight = Screen.height / 15;
         textStyle.fontStyle = FontStyle.Bold;
         DrawOutline(new Rect(gap, screenHeight / 1.8f , screenWidth - gap * 2, height), "Meet our hero, Gusto! He loves his job at the zoo managing the chill chamber and keeping his animal friends cool when it’s too hot outside. They love him, too, and always invite him to play when he comes to work.\n\nToday is the hottest day of summer and the chill chamber is thawing out too quickly. All the animals have broken free and are in danger of overheating!\nGusto is always eager to help but sometimes forgets to take care of himself, too. His battery has almost run dry and he needs to recharge after a scorching summer.\n\nCan you help Gusto balance helping his friends while taking care of himself so he can keep his cool?", textStyle);

         GUIStyle buttonStyle = GUI.skin.GetStyle("Button");
        fontSize = GetFontSize(22);
        buttonStyle.fontSize = fontSize;
         buttonStyle.font = font;
         float buttonWidth = fontSize * 3.7f;
        float buttonHeight = fontSize * 1.5f;
        bool clicked = GUI.Button(new Rect(screenWidth - buttonWidth* 1.2f, screenHeight - buttonHeight* 1.3f, buttonWidth, buttonHeight), "Next", buttonStyle);

        if (clicked)
        {
            source.PlayOneShot(clickSound);
            SetState(GameState.MainMenu);
        }

    }

    void DrawOutline(Rect position, string text, GUIStyle style)
    {
        var outColor = Color.gray;
        var backupStyle = style;
        var oldColor = style.normal.textColor;
        style.normal.textColor = outColor;
        position.x--;
        GUI.Label(position, text, style);
        position.x += 2;
        GUI.Label(position, text, style);
        position.x--;
        position.y--;
        GUI.Label(position, text, style);
        position.y += 2;
        GUI.Label(position, text, style);
        position.y--;
        style.normal.textColor = oldColor;
        GUI.Label(position, text, style);
        style = backupStyle;
    }

 
    void DrawBlackBackGround()
    {
        DrawQuad(new Rect(0, 0, Screen.width, Screen.height), Color.black);
    }

    void DrawQuad(Rect position, Color color)
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, color);
        texture.Apply();
        Texture2D savedSkin = GUI.skin.box.normal.background;
        GUI.skin.box.normal.background = texture;
        GUI.Box(position, GUIContent.none);
        GUI.skin.box.normal.background = savedSkin;
     }

    int GetFontSize(float size)
    {
        float BaseFontScaler = Mathf.Min(Screen.width, Screen.height);
        int fontSize = (int)(2.8f * BaseFontScaler*size / 1920.0f); //scale size font;
        if (fontSize < 14)
            fontSize = 14;
        return fontSize;
    }

}
