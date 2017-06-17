var Gradient : Gradient;
var timeMultiplier:float=1;
var TintColor:boolean=true;
var MainColor:boolean=false;
var loop:boolean=false;

private var curColor:Color;
private var time:float=0.0f;



function Start ()
{

if (TintColor==true) GetComponent.<Renderer>().material.SetColor ("_TintColor", Color(0, 0, 0, 0));
if (MainColor==true) GetComponent.<Renderer>().material.SetColor ("_Color", Color(0, 0, 0, 0));

}


function Update () {
time+=Time.deltaTime*timeMultiplier;
curColor=Gradient.Evaluate(time);

if (TintColor==true) GetComponent.<Renderer>().material.SetColor ("_TintColor", curColor);
if (MainColor==true) GetComponent.<Renderer>().material.SetColor ("_Color", curColor);

if ((loop==true) && (time>=1.0f)) time-=1.0f;
}

