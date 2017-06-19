using UnityEngine;
using System.Collections;

public class textcontrol : MonoBehaviour {

	string txt1=null;
	int i=0;

	void Awake()
	{
        // 0은 텍스트
        // 2는 칼라
        i = csColorPuzzleController.currentMode;
	}

	void Start () 
	{
		char c=gameObject.name[i];
		txt1=c.ToString();
	}
	
	// Update is called once per frame
	void Update () 
	{
		//scaling of gameobject is done according to its position
		if(transform.position.y==252)
		{
			transform.localScale=new Vector2(.4f, .4f);
		}

		if(transform.position.y==250)
		{
			transform.localScale=new Vector2(.6f, .6f);
		}

		//if the object comes in the line of arrows(pointer) shown in screen it will send a message to textinstantiate script on Main Camera
		if(transform.position.y==248)
		{
			transform.localScale=new Vector2(1,1);
			//here the the value of txt is passed via sendmessage function
			GameObject.Find("PuzzleCam(color)").SendMessage("pickme", txt1); 
        }

		//finally after it crosses the final position game object is destroyed
		if(transform.position.y<=-246f)
		{
            transform.localScale = new Vector2(.4f, .4f);
            Destroy(gameObject);
		}

	}
}
