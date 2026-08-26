using UnityEngine;

public class PlayerMoveControls : MonoBehaviour
{
    // ==================================================
    // ค่าความเร็วของตัวละคร
    // ==================================================

    // ความเร็วในการเดินซ้าย-ขวา
    public float speed = 5f;

    // แรงในการกระโดด
    public float jumpForce = 10f;


    // ==================================================
    // ตัวแปรสำหรับตรวจสอบว่าตัวละครอยู่บนพื้นหรือไม่
    // ==================================================

    // ระยะที่ Raycast จะยิงลงไปตรวจสอบพื้น
    // เช่น ถ้าใส่ 0.3 ก็จะตรวจสอบลงไป 0.3 หน่วย
    public float rayLength;

    // Layer ของวัตถุที่เราต้องการให้ถือว่าเป็น "พื้น"
    // เราจะไปเลือก Ground ใน Inspector
    public LayerMask groundLayer;

    // จุดตรวจสอบที่เราเอาไว้ใต้เท้าซ้ายของตัวละคร
    // Unity จะใช้จุดนี้เป็นตำแหน่งเริ่มต้นในการตรวจสอบพื้น
    public Transform leftPoint;
    // จุดตรวจสอบที่เราเอาไว้ใต้เท้าขวาของตัวละคร
    // Unity จะใช้จุดนี้เป็นตำแหน่งเริ่มต้นในการตรวจสอบพื้น
    public Transform rightPoint;

    // เก็บสถานะของตัวละครว่าอยู่บนพื้นหรือไม่
    // true  = อยู่บนพื้น
    // false = ไม่ได้อยู่บนพื้น / กำลังกระโดด
    private bool grounded = false;


    // ==================================================
    // ตัวแปรสำหรับรับ Input และควบคุม Rigidbody
    // ==================================================

    // รับข้อมูลการกดปุ่มจาก GatherInput
    private GatherInput gatherInput;

    // ใช้ควบคุมการเคลื่อนที่ทางฟิสิกส์ของตัวละคร
    private Rigidbody2D rigidbody2D;


    // ==================================================
    // ตัวแปรสำหรับควบคุม Animation
    // ==================================================

    private Animator animator;


    // ==================================================
    // ตัวแปรสำหรับตรวจสอบทิศทางที่ตัวละครหัน
    // ==================================================

    // 1  = หันขวา
    // -1 = หันซ้าย
    private int direction = 1;


    // ==================================================
    // Start
    // ==================================================

    void Start()
    {
        // รับ Component GatherInput จาก Player
        gatherInput = GetComponent<GatherInput>();

        // รับ Component Rigidbody2D จาก Player
        rigidbody2D = GetComponent<Rigidbody2D>();

        // รับ Component Animator จาก Player
        animator = GetComponent<Animator>();

        // ตรวจสอบว่าตอนเริ่มเกมตัวละครหันไปทางไหน
        if (transform.localScale.x < 0)
        {
            direction = -1;
        }
    }


    // ==================================================
    // FixedUpdate
    // ==================================================

    private void FixedUpdate()
    {
        // ตรวจสอบก่อนว่าตอนนี้ตัวละครอยู่บนพื้นหรือไม่
        // ผลลัพธ์จะถูกเก็บไว้ในตัวแปร grounded
        CheckStatus();

        // เรียกระบบเดินซ้าย-ขวา
        Move();

        // เรียกระบบกระโดด
        // ระบบนี้จะเช็ก grounded อีกทีว่าอยู่บนพื้นหรือไม่
        JumpPlayer();

        // อัปเดตค่าของ Animation
        SetAnimatorValues();
    }


    // ==================================================
    // ระบบตรวจสอบสถานะการอยู่บนพื้น
    // ==================================================

    private void CheckStatus()
    {

        // วาดเส้น Raycast จาก LeftPoint ลงด้านล่าง
        Debug.DrawRay(
            leftPoint.position,
            Vector2.down * rayLength,
            Color.red
        );

        // วาดเส้น Raycast จาก RightPoint ลงด้านล่าง
        Debug.DrawRay(
            rightPoint.position,
            Vector2.down * rayLength,
            Color.blue
        );

        // ตรวจสอบพื้นจากจุดใต้เท้าซ้าย
        RaycastHit2D leftCheckHit = Physics2D.Raycast(
            leftPoint.position,
            Vector2.down,
            rayLength,
            groundLayer
        );

        // ตรวจสอบพื้นจากจุดใต้เท้าขวา
        RaycastHit2D rightCheckHit = Physics2D.Raycast(
            rightPoint.position,
            Vector2.down,
            rayLength,
            groundLayer
        );

        // ถ้าจุดใดจุดหนึ่งตรวจเจอพื้น
        // ให้ถือว่าตัวละครอยู่บนพื้น
        grounded = leftCheckHit || rightCheckHit;
    }


    // ==================================================
    // ระบบการเดิน
    // ==================================================

    private void Move()
    {
        // ตรวจสอบว่าต้องหันซ้ายหรือขวาหรือไม่
        Flip();

        // กำหนดความเร็วของ Player
        // valueX เป็นค่าจากการกดปุ่มซ้าย-ขวา
        rigidbody2D.linearVelocity = new Vector2(
            speed * gatherInput.valueX,
            rigidbody2D.linearVelocity.y
        );
    }


    // ==================================================
    // ระบบการกระโดด
    // ==================================================

    private void JumpPlayer()
    {
        // ตรวจสอบ 2 เงื่อนไขพร้อมกัน
        //
        // 1. ผู้เล่นต้องกดปุ่มกระโดด
        // 2. ตัวละครต้องอยู่บนพื้น
        //
        // && หมายถึง "และ"
        //
        // ดังนั้นถ้ากำลังกระโดดอยู่บนอากาศ
        // grounded จะเป็น false
        // ทำให้ไม่สามารถกระโดดซ้ำกลางอากาศได้
        if (gatherInput.jumpInput && grounded)
        {
            // ถ้ากดกระโดดและอยู่บนพื้น
            // ให้ตัวละครมีความเร็วขึ้นด้านบนตามค่า jumpForce
            rigidbody2D.linearVelocity = new Vector2(
                gatherInput.valueX * speed,
                jumpForce
            );
        }

        // รีเซ็ตค่าการกดกระโดด
        // เพื่อไม่ให้ตัวละครกระโดดซ้ำจากการกดครั้งเดิม
        gatherInput.jumpInput = false;
    }


    // ==================================================
    // ระบบ Animation
    // ==================================================

    private void SetAnimatorValues()
    {
        // นำความเร็วแกน X ไปใส่ใน Parameter
        // ชื่อ Speed ของ Animator
        //
        // Mathf.Abs() ทำให้ค่าติดลบกลายเป็นค่าบวก
        // เช่น -5 จะกลายเป็น 5
        animator.SetFloat(
            "Speed",
            Mathf.Abs(rigidbody2D.linearVelocity.x)
        );
    }


    // ==================================================
    // ระบบหันซ้าย / ขวา
    // ==================================================

    private void Flip()
    {
        // ถ้าเดินไปทางขวา
        // และตัวละครกำลังหันซ้ายอยู่
        if (gatherInput.valueX > 0 && direction < 0)
        {
            // พลิกตัวละครกลับมาทางขวา
            transform.localScale = new Vector3(
                Mathf.Abs(transform.localScale.x),
                transform.localScale.y,
                transform.localScale.z
            );

            // บันทึกว่าตอนนี้หันขวา
            direction = 1;
        }

        // ถ้าเดินไปทางซ้าย
        // และตัวละครกำลังหันขวาอยู่
        else if (gatherInput.valueX < 0 && direction > 0)
        {
            // พลิกตัวละครไปทางซ้าย
            transform.localScale = new Vector3(
                -Mathf.Abs(transform.localScale.x),
                transform.localScale.y,
                transform.localScale.z
            );

            // บันทึกว่าตอนนี้หันซ้าย
            direction = -1;
        }
    }
}


 