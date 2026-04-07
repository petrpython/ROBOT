using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RobotMotor : MonoBehaviour
{
    [Header("Настройки моторов")]
    public float maxSpeed = 5f;
    public float maxRotationSpeed = 200f;
    public float motorForce = 10f;

    public Transform leftWheel;
    public Transform rightWheel;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogError("❌ ОШИБКА: Rigidbody2D не найден на " + gameObject.name);
        }
        else
        {
            Debug.Log("✅ RobotMotor запущен! Rigidbody2D найден.");
            Debug.Log("🔧 Body Type: " + rb.bodyType);
            Debug.Log("🔧 Mass: " + rb.mass);
        }
    }

    void Update()
    {
        // WASD ОТКЛЮЧЕН
    }

    void FixedUpdate()
    {
        // Физика только через методы
    }

    public void SetManualControl(bool enabled)
    {
        // Для совместимости
    }

    public void Move(float force)
    {
        Debug.Log("🔧 RobotMotor.Move() force=" + force);

        if (rb == null)
        {
            Debug.LogError("❌ Rigidbody2D не найден!");
            return;
        }

        // ✅ ИСПОЛЬЗУЕМ velocity вместо AddForce
        Vector2 moveDirection = transform.up * force * maxSpeed;
        rb.linearVelocity = moveDirection;

        Debug.Log("🔧 Установлена скорость: " + rb.linearVelocity);
    }

    public void Rotate(float torque)
    {
        Debug.Log("🔧 RobotMotor.Rotate() torque=" + torque);

        if (rb == null)
        {
            Debug.LogError("❌ Rigidbody2D не найден!");
            return;
        }

        // ✅ Поворот через изменение угла
        float rotationAmount = torque * maxRotationSpeed * Time.fixedDeltaTime;
        transform.Rotate(0, 0, rotationAmount);

        Debug.Log("🔧 Угол поворота: " + transform.eulerAngles.z);
    }

    public void Stop()
    {
        Debug.Log("🛑 RobotMotor.Stop()");

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0;
        }
    }
}