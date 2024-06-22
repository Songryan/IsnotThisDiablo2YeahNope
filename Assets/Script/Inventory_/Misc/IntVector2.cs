[System.Serializable] // 구조체를 직렬화 가능하도록 설정
public struct IntVector2
{
    public int x, y; // x와 y 좌표를 나타내는 변수

    public IntVector2(int num1, int num2) // 생성자: 두 개의 정수를 받아 IntVector2를 초기화
    {
        x = num1;
        y = num2;
    }

    public string String // 벡터를 문자열로 반환하는 속성
    {
        get
        {
            return ("(" + x + ", " + y + ")"); // "(x, y)" 형식으로 반환
        }
    }

    public static IntVector2 One // (1, 1) 벡터를 반환하는 속성
    {
        get { return new IntVector2(1, 1); }
    }
    public static IntVector2 OneNeg // (-1, -1) 벡터를 반환하는 속성
    {
        get { return new IntVector2(-1, -1); }
    }
    public static IntVector2 Zero // (0, 0) 벡터를 반환하는 속성
    {
        get { return new IntVector2(0, 0); }
    }
    public static IntVector2 Up // (0, 1) 벡터를 반환하는 속성
    {
        get { return new IntVector2(0, 1); }
    }
    public static IntVector2 Down // (0, -1) 벡터를 반환하는 속성
    {
        get { return new IntVector2(0, -1); }
    }
    public static IntVector2 Left // (-1, 0) 벡터를 반환하는 속성
    {
        get { return new IntVector2(-1, 0); }
    }
    public static IntVector2 Right // (1, 0) 벡터를 반환하는 속성
    {
        get { return new IntVector2(1, 0); }
    }

    public static IntVector2 operator +(IntVector2 a, IntVector2 b) // 두 벡터를 더하는 연산자 오버로딩
    {
        return new IntVector2(a.x + b.x, a.y + b.y);
    }
    public static IntVector2 operator +(IntVector2 a, int b) // 벡터에 정수를 더하는 연산자 오버로딩
    {
        return new IntVector2(a.x + b, a.y + b);
    }
    public static IntVector2 operator -(IntVector2 a, IntVector2 b) // 두 벡터를 빼는 연산자 오버로딩
    {
        return new IntVector2(a.x - b.x, a.y - b.y);
    }
    public static IntVector2 operator -(IntVector2 a, int b) // 벡터에서 정수를 빼는 연산자 오버로딩
    {
        return new IntVector2(a.x - b, a.y - b);
    }
    public static IntVector2 operator *(IntVector2 a, int b) // 벡터에 정수를 곱하는 연산자 오버로딩
    {
        return new IntVector2(a.x * b, a.y * b);
    }
    public static IntVector2 operator /(IntVector2 a, int b) // 벡터를 정수로 나누는 연산자 오버로딩
    {
        return new IntVector2(a.x / b, a.y / b);
    }

    public static int Area(IntVector2 a) // 벡터의 면적을 계산하는 메서드
    {
        return (a.x * a.y);
    }
    public static float Slope(IntVector2 a) // 벡터의 기울기를 계산하는 메서드
    {
        return ((float)a.y / (float)a.x);
    }
    public static void Swap(ref IntVector2 a) // 벡터의 x와 y 값을 교환하는 메서드
    {
        int temp = a.x;
        a.x = a.y;
        a.y = temp;
    }

    public static bool operator ==(IntVector2 a, IntVector2 b) // 두 벡터가 같은지 비교하는 연산자 오버로딩
    {
        if ((a.x == b.x) && (a.y == b.y))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public static bool operator !=(IntVector2 a, IntVector2 b) // 두 벡터가 다른지 비교하는 연산자 오버로딩
    {
        if ((a.x != b.x) || (a.y != b.y))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool OrGreater(IntVector2 a, IntVector2 b) // 벡터 a의 x 또는 y가 벡터 b의 x 또는 y보다 큰지 비교
    {
        if (a.x > b.x || a.y > b.y)
        {
            return true;
        }
        return false;
    }
    public static bool OrGreater(IntVector2 a, int b) // 벡터 a의 x 또는 y가 정수 b보다 큰지 비교
    {
        if (a.x > b || a.y > b)
        {
            return true;
        }
        return false;
    }
    public static bool OrLesser(IntVector2 a, IntVector2 b) // 벡터 a의 x 또는 y가 벡터 b의 x 또는 y보다 작은지 비교
    {
        if (a.x < b.x || a.y < b.y)
        {
            return true;
        }
        return false;
    }
    public static bool OrLesser(IntVector2 a, int b) // 벡터 a의 x 또는 y가 정수 b보다 작은지 비교
    {
        if (a.x < b || a.y < b)
        {
            return true;
        }
        return false;
    }

    public override bool Equals(object o) // 오버라이드된 Equals 메서드 (기본 구현, 실제 비교는 == 연산자가 처리)
    {
        return true;
    }

    public override int GetHashCode() // 오버라이드된 GetHashCode 메서드 (기본 구현, 필요에 따라 변경 가능)
    {
        return 0;
    }
}
