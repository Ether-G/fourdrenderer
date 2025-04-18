# 4D Object Renderer

A C# Windows Forms application for visualizing four-dimensional (4D) geometric objects by projecting them into 3D space and then onto a 2D screen.

## How it Works

The application renders 4D objects using a projection pipeline:

1.  **4D Coordinates:** Objects are defined by vertices in 4D space (X, Y, Z, W).
2.  **4D to 3D Projection:** A perspective projection is used to transform the 4D coordinates into 3D space. This is analogous to how a 3D scene is projected onto a 2D camera sensor, but with an extra dimension. The distance of the viewpoint along the W-axis affects the perspective.
3.  **3D to 2D Projection:** The resulting 3D coordinates are then projected onto a 2D plane using standard perspective projection, similar to typical 3D graphics rendering.
4.  **Rendering:** The final 2D points are used to draw the wireframe edges of the object on the screen.

## Features

* Visualize various 4D objects (Tesseract, Hypersphere, Pentachoron, Toratope).
* Wireframe rendering with color-coded edges often indicating axes or structure.
* Interactive rotation of objects across the 6 distinct planes in 4D (XY, XZ, XW, YZ, YW, ZW).
* Camera controls to move the viewpoint in 4D space.
* Ability to switch between different objects in the scene.

## Architecture

```mermaid
classDiagram
    %% Mathematics Framework
    class Vector2D {
        +float X, Y
        +Point ToPoint()
        <<helper>>
    }
    class Vector3D {
        +float X, Y, Z
        +Vector2D ProjectTo2D(float viewerDistance)
        <<helper>>
    }
    class Vector4D {
        +float X, Y, Z, W
        +Vector3D ProjectTo3D(float viewerDistance)
        <<helper>>
    }
    class Matrix4D {
        +float[,] Matrix
        +Vector4D Transform(Vector4D)
        +static Matrix4D CreateRotationXY(float)
        +static Matrix4D CreateRotationXZ(float)
        +static Matrix4D CreateRotationXW(float)
        +static Matrix4D CreateRotationYZ(float)
        +static Matrix4D CreateRotationYW(float)
        +static Matrix4D CreateRotationZW(float)
        <<helper>>
    }

    %% 4D Objects
    class Object4D {
        <<abstract>>
        +List~Vector4D~ Vertices
        +List~Edge4D~ Edges
        +Matrix4D Transformation
        +string Name
        +Vector4D Center
        +void ApplyTransformation(Matrix4D)
        +abstract void GenerateGeometry()
        +void Render(Renderer)
        #virtual void RenderDetails(Renderer, List~Vector2D~, List~Vector3D~)
    }
    class Tesseract { +float Size }
    class Hypersphere { +float Radius; +int Resolution }
    class Pentachoron { +float Size }
    class Toratope { +float MajorRadius; +float MinorRadius; +int Resolution }
    class Edge4D { +int StartVertexIndex; +int EndVertexIndex; +Color Color }

    %% Rendering System
    class Camera4D {
        +Vector4D Position
        +float ViewerDistance
        +float Screen3DDistance
        +Vector3D ProjectTo3D(Vector4D)
        +Vector2D ProjectTo2D(Vector3D)
    }
    class Renderer {
        +Bitmap Canvas
        +Camera4D Camera
        +void DrawLine(Vector2D, Vector2D, Color)
        +void DrawPoint(Vector2D, Color, int)
        +void RenderObject(Object4D)
    }

    %% Scene Management
    class Scene4D {
        +List~Object4D~ Objects
        +Object4D SelectedObject
        +Camera4D Camera
        +void AddObject(Object4D)
        +void SelectObject(int)
        +void ApplyRotation(Matrix4D)
        +void Render(Renderer)
    }
    class Engine4D {
        +Scene4D Scene
        +Renderer Renderer
        +float[] RotationAngles
        +boolean[] ActiveRotations
        +void Update(float)
        +void RotateObjects(float)
        +void ProcessInput(Keys, boolean)
        +void Render()
    }

    %% UI
    class Form1 {
      -Engine4D _engine
      -PictureBox _renderSurface
      <<UI Form>>
    }

    %% Relationships
    Vector4D --> Vector3D : projects to
    Vector3D --> Vector2D : projects to

    Object4D <|-- Tesseract
    Object4D <|-- Hypersphere
    Object4D <|-- Pentachoron
    Object4D <|-- Toratope

    Object4D *-- "many" Vector4D : vertices
    Object4D *-- "many" Edge4D : edges
    Object4D o-- Matrix4D : transformation

    Edge4D ..> Renderer : uses
    Edge4D ..> Vector2D : uses

    Renderer o-- Camera4D
    Renderer ..> Object4D : renders

    Scene4D *-- "many" Object4D
    Scene4D *-- "1" Camera4D

    Engine4D *-- "1" Scene4D
    Engine4D *-- "1" Renderer
    Engine4D ..> Object4D : applies rotation to selected
    Engine4D ..> Matrix4D : creates rotation

    Form1 *-- "1" Engine4D
    Form1 ..> Renderer : displays canvas from
```

## Object Previews

**Tesseract (Hypercube)**

![4d](https://github.com/user-attachments/assets/5399b242-8abf-42e3-bee0-4a28707fcc0c)

**Hypersphere (3-Sphere)**

![hypersphere2](https://github.com/user-attachments/assets/adb60369-38c5-41ab-8b70-6a2bc44b08e2)

**Pentachoron (5-Cell)**

![5cellvertrack4](https://github.com/user-attachments/assets/e81f7c91-1518-41e8-8333-681b1f5ce6ad)

**Toratope (4D Torus)**

![Toratope](https://github.com/user-attachments/assets/ab5ccbe3-5a39-4bb2-85f3-a175d8952f90)

## Controls

* **1-6 Keys**: Toggle rotation in specific 4D planes (1:XY, 2:XZ, 3:XW, 4:YZ, 5:YW, 6:ZW).
* **Space**: Pause/resume animation.
* **Tab**: Switch selected object.
* **W/S**: Move camera along Y-axis.
* **A/D**: Move camera along X-axis.
* **Q/E**: Move camera along Z-axis.
* **R/F**: Move camera along W-axis.
* **+/- (or OemPlus/OemMinus)**: Adjust 4D->3D viewer distance.
* **Up/Down Arrows**: Increase/decrease rotation speed.
* **T**: Toggle rotation mode (Reset Each Frame / Cumulative).

## Requirements

* .NET 8.0 (or compatible).
* Windows Operating System (uses Windows Forms).

## How to Run

1.  Clone the repository.
2.  Open the solution file (`.sln`) in Visual Studio or use the .NET CLI.
3.  Build the solution (`dotnet build`).
4.  Run the application (`dotnet run --project FourDRenderer/FourDRenderer.csproj`).
