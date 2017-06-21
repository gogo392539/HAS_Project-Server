using UnityEngine;
using System.Collections;


// this script is similar to the "textinstantiate" script only have a little changes...
//everything you need know is decribed in another script
public class textinstantiate1 : MonoBehaviour {

    public GameObject manager;
	public GameObject[] text;
	GameObject[] clone;
	public GUIStyle[] styles;
	string textcolor="p";
	public GUIText score, gtime;
	int count=0;
	public GameObject w1, w2;
	bool flag=false;
	string txt="t";
	public GUITexture[] colors;
	public Animation[] anim;
	float time=31;
	public GameObject commongui;
	public GUITexture[] common;

    private string t_score;
    private string t_time;

    public void StartPuzzle()
    {
        Time.timeScale = 1;

        PlayerPrefs.SetInt("timetrial", 1);
        PlayerPrefs.SetInt("gameover", 0);
        PlayerPrefs.SetInt("pause", 2);
        PlayerPrefs.Save();

        commongui.SetActive(true);

        InvokeRepeating("create", 0, 1f);
        InvokeRepeating("fun", 0, 1f);

        count = 0;
        time = 31;
    }

    private void Start()
    {
        t_score = score.text;
        t_time = gtime.text;
    }

    void fun()
	{
		if(flag)
		{
			w1.SetActive(false);
			w2.SetActive(false);
			flag=false;
		}
		else
		{
			w1.SetActive(true);
			w2.SetActive(true);
			flag=true;
		}

		if(PlayerPrefs.GetInt("gameover")!=1)
		{
			time--;
            gtime.text = t_time + " " + time;
		}
	}

	void create()
	{
        if (time != 0)
        {
            Instantiate(text[Random.Range(0, text.Length)], new Vector3(0, 254, 0), Quaternion.identity);

            clone = GameObject.FindGameObjectsWithTag("text");
            foreach (GameObject g in clone)
            {
                g.transform.position = new Vector2(0, g.transform.position.y - 2f);
            }
        }
	}

	void pickme(string str1)
	{
		textcolor=str1;
	}

	void remove()
	{
		clone=GameObject.FindGameObjectsWithTag("text");
		foreach(GameObject g in clone)
		{
			if(g.transform.position.y==248f)
			{
				count++;
                score.text = t_score + " " + count;
				Destroy(g);
			}
		}
	}

	void Update()
	{
		if(PlayerPrefs.GetInt("pause")==0)
		{
			Time.timeScale=1;
		}	

		if(Input.GetMouseButtonDown(0))
		{
			//Vector2 pos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
			//RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(pos), Vector2.zero);

			if (common[0].HitTest(Input.mousePosition, Camera.main)) {
                GetComponent<AudioSource>().Play();
                txt ="b";
				anim[0].Play();
			}
		    if (common[1].HitTest(Input.mousePosition, Camera.main)) {
                GetComponent<AudioSource>().Play();
                txt ="c";
				anim[1].Play();
			}
			if (common[2].HitTest(Input.mousePosition, Camera.main)) {
                GetComponent<AudioSource>().Play();
                txt ="g";
				anim[2].Play();
			}
			if (common[3].HitTest(Input.mousePosition, Camera.main)) {
                GetComponent<AudioSource>().Play();
                txt ="m";
				anim[3].Play();
			}
			if (common[4].HitTest(Input.mousePosition, Camera.main)) {
                GetComponent<AudioSource>().Play();
                txt ="o";
				anim[4].Play();
			}
			if (common[5].HitTest(Input.mousePosition, Camera.main)) {
                GetComponent<AudioSource>().Play();
                txt ="r";
				anim[5].Play();
			}
			if (common[6].HitTest(Input.mousePosition, Camera.main)) {
                GetComponent<AudioSource>().Play();
                txt ="v";
				anim[6].Play();
			}
			if (common[7].HitTest(Input.mousePosition, Camera.main)) {
                GetComponent<AudioSource>().Play();
                txt ="y";
				anim[7].Play();
			}
        }

		if(Input.GetMouseButtonUp(0))
		{
			txt="a";		
		}

        if (count == 15)
        {
            PlayerPrefs.SetInt("gameover", 1);
            deleteAlltext();
            manager.GetComponent<csColorPuzzleController>().ctrlCompleteButton(true);
        }

        if (time==0)
		{
			PlayerPrefs.SetInt("gameover", 1);
            deleteAlltext(); 
        }   
    }

	//farzi is hindi word...means useless but this one is used
	void farzi()
	{
		Time.timeScale=0;
	}

	void FixedUpdate()
	{
		if(textcolor.Equals(txt))
		{
			remove();
		}
	}

    public void deleteAlltext()
    {
        var texts = GameObject.FindGameObjectsWithTag("text");
        foreach (var clone in texts)
            Destroy(clone);
        finishPuzzle();
    }

    public void finishPuzzle()
    {
        CancelInvoke("create");
        CancelInvoke("fun");
    }
}
