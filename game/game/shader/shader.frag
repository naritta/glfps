#version 330 core
precision mediump float;
out vec4 FragColor;
uniform vec2 resolution;
in vec3 ourColor;

void main()
{
//    FragColor = vec4(ourColor, 1.0f);
    FragColor = vec4(0.9, 0.0, 0.0, 1.0f);
//    if(gl_FragCoord.x/resolution.x < 0.5){
//        FragColor = vec4(1.f, 0.f, 0.f, 1.f);
//    }else{
//        FragColor = vec4(1.f, 1.f, 1.f, 1.f);
//    }
}

//float metaball(vec2 pos, vec2 offset, float scale){
//    // ポジション修正
//    pos = pos - offset;
//    float len = length(pos);
//    float draw = 0.0;
//    // 円を描く範囲
//    if(len < scale){
//        draw = (1.0 - len / scale);
//    }
//    return draw;
//}
//
//
//void main(){
//    // gl_FragCoord.xy = 0〜512 → *2.0で0〜1024 → -resoで-512〜512 → /resoで-1.0〜1.0座標
//    vec2 p = (gl_FragCoord.xy * 2.0 - resolution) / resolution;
//    float ball = 0.0;
//    float speed = 10.0; // 炎のスピード
//    float yPos = 0.8;   // Y軸の調整
//
//    // 横揺れの炎8つ
//    for(int i = 1; i <= 8; i++){
//        float x = float(i) * 1.0;
//        float y = float(8-i) * 0.18;
//        float moveX = cos(x+0*speed) * 0.31;
//        float moveY = y - yPos;
//        float scale = 0.56;
//        // メタボール生成
//        ball += metaball(p, vec2(moveX, moveY), scale);
//    }
//    // blue color
//    FragColor = vec4(vec3(ball*0.25+ball*0.1, ball*0.45+ball*0.1, ball), 1.0);
//}
