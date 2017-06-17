#pragma strict
//var overTime:float=1;
private var time:float=0;
var overTime:float=1;

var CurveX : AnimationCurve;

private var curX:float;
var CurveY : AnimationCurve;

private var curY:float;
var CurveZ : AnimationCurve;

private var curZ:float;

var local:boolean=true;

function Start () {

}

function Update () {

time+=Time.deltaTime/overTime;
curX=CurveX.Evaluate(time);
curY=CurveY.Evaluate(time);
curZ=CurveZ.Evaluate(time);

if (local==true){
transform.Translate(Vector3(curX,curY,curZ)*Time.deltaTime);
}

if (local==false){
transform.Translate(Vector3(curX,curY,curZ)*Time.deltaTime, Space.World);
}


}