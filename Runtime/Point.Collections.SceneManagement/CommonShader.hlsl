#ifndef COMMONSHADER_INCLUDED
#define COMMONSHADER_INCLUDED

// https://docs.unity3d.com/Packages/com.unity.shadergraph@6.7/manual/Custom-Function-Node.html
// https://stackoverflow.com/questions/12030101/an-outline-sharp-transition-in-a-fragment-shader
void outline(float t, float threshold, float width, out float result){
    result = clamp(width - abs(threshold - t) / fwidth(t), 0.0, 1.0);
}
//float outlineEuclidean(float t, float threshold, float width)
//{
//    float dx = dFdx(t);
//    float dy = dFdy(t);
//    float ewidth = sqrt(dx * dx + dy * dy);
//    return clamp(width - abs(threshold - t) / ewidth, 0.0, 1.0);
//}

#endif // COMMONSHADER_INCLUDED