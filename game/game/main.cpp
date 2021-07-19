#include <GL/glew.h>

#include <GLFW/glfw3.h>
#include "shader.hpp"
#include <iostream>

#include "glm/glm.hpp"
#include "glm/gtc/matrix_transform.hpp"
#include "glm/gtx/norm.hpp"

#include <iostream>


using namespace glm;

void framebuffer_size_callback(GLFWwindow* window, int width, int height);
void processInput(GLFWwindow *window);
void checkCollision(float bullet_position[], float target_position[]);
void key_callback(GLFWwindow* window, int key, int scancode, int action, int mode);
void processInput(float dt, float bullet_position[]);

// settings
const unsigned int SCR_WIDTH = 500;
const unsigned int SCR_HEIGHT = 500;

bool keys[1024];
bool keysProcessed[1024];

using namespace std;

int main()
{
    // glfw: initialize and configure
    // ------------------------------
    glfwInit();
    glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 3);
    glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 3);
    glfwWindowHint(GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE);
    
#ifdef __APPLE__
    glfwWindowHint(GLFW_OPENGL_FORWARD_COMPAT, GL_TRUE);
#endif
    
    // glfw window creation
    // --------------------
    GLFWwindow* window = glfwCreateWindow(SCR_WIDTH, SCR_HEIGHT, "LearnOpenGL", NULL, NULL);
    if (window == NULL)
    {
        std::cout << "Failed to create GLFW window" << std::endl;
        glfwTerminate();
        return -1;
    }
    glfwMakeContextCurrent(window);
    glfwSetKeyCallback(window, key_callback);
    glfwSetFramebufferSizeCallback(window, framebuffer_size_callback);
    
    // Initialize GLEW
    glewExperimental = true; // Needed for core profile
    if (glewInit() != GLEW_OK) {
        fprintf(stderr, "Failed to initialize GLEW\n");
        getchar();
        glfwTerminate();
        return -1;
    }
    
    // build and compile our shader program
    // ------------------------------------
    ////    Shader ourShader("shader.vert", "shader.frag"); // you can name your shader files however you like
    //    Shader ourShader("/Users/ritta/Desktop/shader/game/game/shader.vert", "/Users/ritta/Desktop/shader/game/game/shader.frag"); // you can name your shader files however you like
    // Create and compile our GLSL program from the shaders
    GLuint shaderID = LoadShaders("/Users/ritta/glfps/game/game/board.vert", "/Users/ritta/glfps/game/game/raymarch.frag");
    //    GLuint shaderID = LoadShaders("shader.vert", "shader.frag");
    
    // set up vertex data (and buffer(s)) and configure vertex attributes
    // ------------------------------------------------------------------
    //    float vertices[] = {
    //        // positions         // colors
    //        1.0f, -1.0f, 0.0f,  1.0f, 0.0f, 0.0f,  // bottom right
    //        -1.0f, 1.0f, 0.0f,  0.0f, 1.0f, 0.0f,  // bottom left
    //        1.0f,  1.0f, 0.0f,  0.0f, 0.0f, 1.0f,  // top
    //    };
    float positions_vertices[] = {
        // positions
        -1.0f, -1.0f, 0.0f,
        1.0f, -1.0f, 0.0f,
        -1.0f,  1.0f, 0.0f,
        1.0f,  1.0f, 0.0f,
    };
    
    float bullet_position[] = {
        0.0f, 0.0f, 0.0f, 0.6f
    };
    
    float target_position[] = {
        0.5f, 0.0f, -10.0f, 0.6f
    };
    
    unsigned int positions_VBO, colors_VBO, VAO;
    glGenVertexArrays(1, &VAO);
    glBindVertexArray(VAO);
    
    glGenBuffers(1, &positions_VBO);
    glBindBuffer(GL_ARRAY_BUFFER, positions_VBO);
    glBufferData(GL_ARRAY_BUFFER, sizeof(positions_vertices), positions_vertices, GL_STREAM_DRAW);
    
    // position attribute
    glBindBuffer(GL_ARRAY_BUFFER, positions_VBO);
    glEnableVertexAttribArray(0);
    glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 0, (void*)0);
    
    // You can unbind the VAO afterwards so other VAO calls won't accidentally modify this VAO, but this rarely happens. Modifying other
    // VAOs requires a call to glBindVertexArray anyways so we generally don't unbind VAOs (nor VBOs) when it's not directly necessary.
    // glBindVertexArray(0);
    
    // variables for timing
    double prevTime = glfwGetTime();
    double currTime = 0;
    double deltaTime = 0;
    double speed = 2.0;
    unsigned int frames = 0;
    double fpsInterval = 0;
    
    float bulletZ = 0.0f;
    
    // render loop
    // -----------
    while (!glfwWindowShouldClose(window))
    {
        // update delta time
        currTime = glfwGetTime();
        deltaTime = currTime - prevTime;
        prevTime = currTime;
        //        cout << currTime << endl;
        
        //        vertices[2]-=deltaTime*0.1;
        //        vertices[8]-=deltaTime*0.1;
        //        vertices[14]-=deltaTime*0.1;
        
        //        glBindBuffer(GL_ARRAY_BUFFER, VBO);
        //        glBufferData(GL_ARRAY_BUFFER, sizeof(vertices), vertices, GL_STATIC_DRAW);
        
        // position attribute
        //        glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 6 * sizeof(float), (void*)0);
        //        glEnableVertexAttribArray(0);
        
        // input
        // -----
        processInput(window);
        
        // render
        // ------
        //        glClearColor(1.0f, 1.0f, 1.0f, 0.0f);
        glClearColor(0.0f, 0.0f, 0.0f, 0.0f);
        glClear(GL_COLOR_BUFFER_BIT);
        
        // render the triangle
        glUseProgram(shaderID);
        glBindVertexArray(VAO);
        glDrawArrays(GL_TRIANGLE_STRIP, 0, 4);
        
        processInput(deltaTime, bullet_position);
        
        bullet_position[2] -= deltaTime*speed;
        
        checkCollision(bullet_position, target_position);
        
        glUniform2f(glGetUniformLocation(shaderID, "resolution"), SCR_WIDTH, SCR_WIDTH);
        glUniform4f(glGetUniformLocation(shaderID, "bullet"), bullet_position[0], bullet_position[1], bullet_position[2], bullet_position[3]);
        glUniform4f(glGetUniformLocation(shaderID, "target"), target_position[0], target_position[1], target_position[2], target_position[3]);
        
        // glfw: swap buffers and poll IO events (keys pressed/released, mouse moved etc.)
        // -------------------------------------------------------------------------------
        glfwSwapBuffers(window);
        glfwPollEvents();
    }
    
    // optional: de-allocate all resources once they've outlived their purpose:
    // ------------------------------------------------------------------------
    glDeleteVertexArrays(1, &VAO);
    glDeleteBuffers(1, &positions_VBO);
    glDeleteProgram(shaderID);
    
    // glfw: terminate, clearing all previously allocated GLFW resources.
    // ------------------------------------------------------------------
    glfwTerminate();
    return 0;
}

void key_callback(GLFWwindow* window, int key, int scancode, int action, int mode)
{
    if (key == GLFW_KEY_ESCAPE && action == GLFW_PRESS)
        glfwSetWindowShouldClose(window, true);
    if (key >= 0 && key < 1024)
    {
        if (action == GLFW_PRESS){
            keys[key] = true;
        } else if (action == GLFW_RELEASE) {
            keys[key] = false;
            keysProcessed[key] = false;
        }
    }
}

void checkCollision(float bullet_position[], float target_position[]){
    float d = pow(bullet_position[0]-target_position[0], 2.0) + pow(bullet_position[1]-target_position[1], 2.0) + pow(bullet_position[2]-target_position[2], 2.0);
    if (d < pow((bullet_position[3]+target_position[3]), 2.0)) {
        // delete by moving for the time being
        target_position[1] = 10;
    };
}

void processInput(float dt, float bullet_position[])
{
    float basicVel = 2.0;
    float velocity = basicVel * dt;
    if (keys[GLFW_KEY_RIGHT]) {
        bullet_position[0] += velocity;
    } else if (keys[GLFW_KEY_LEFT]) {
        bullet_position[0] -= velocity;
    } else if (keys[GLFW_KEY_UP]) {
        bullet_position[1] += velocity;
    } else if (keys[GLFW_KEY_DOWN]) {
        bullet_position[1] -= velocity;
    }
}

// process all input: query GLFW whether relevant keys are pressed/released this frame and react accordingly
// ---------------------------------------------------------------------------------------------------------
void processInput(GLFWwindow *window)
{
    if (glfwGetKey(window, GLFW_KEY_ESCAPE) == GLFW_PRESS)
    glfwSetWindowShouldClose(window, true);
}

// glfw: whenever the window size changed (by OS or user resize) this callback function executes
// ---------------------------------------------------------------------------------------------
void framebuffer_size_callback(GLFWwindow* window, int width, int height)
{
    // make sure the viewport matches the new window dimensions; note that width and
    // height will be significantly larger than specified on retina displays.
    glViewport(0, 0, width, height);
}
