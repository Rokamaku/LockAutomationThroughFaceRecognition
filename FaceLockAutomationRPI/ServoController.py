import RPi.GPIO as GPIO
import time
  
GPIO.setmode(GPIO.BCM)
GPIO.setup(17, GPIO.OUT)
GPIO.setup(19, GPIO.OUT)

 
servo=GPIO.PWM(19, 50)
GPIO.output(17, GPIO.LOW)
# Tạo xung PWM trên chan vật lý 7 với tần số 50Hz --> chu kì 20ms
 
try:
#    while True:
          servo.start(2) 
          # Khởi động xuất pwm với Duty Cycle là 2%
          # --> Đô rộng xung là 2% x 20ms = 0,4ms => đây là độ rộng xung 
          # thực tế đễ servo quay đến góc 0 độ
#          time.sleep(1) # Dừng 1 giây để motor quay đến vị trí
#          servo.ChangeDutyCycle(4)
#          time.sleep(1)
#          servo.ChangeDutyCycle(6)
          time.sleep(2)
          servo.ChangeDutyCycle(7.5)
          time.sleep(3)
#          servo.ChangeDutyCycle(10)
#          time.sleep(1)
#          servo.ChangeDutyCycle(12)
          # Thay đổi Duty Cycle tới 12%
          # --> Đô rộng xung là 12% x 20ms = 2,4ms =>; đây là độ rộng xung 
          # thực tế đễ servo quay đến góc 180 độ
#          time.sleep(1)
          servo.stop()
except KeyboardInterrupt:
#    GPIO.output(15, GPIO.HIGH)
#    GPIO.cleanup()
    print("Control Relay stopped")
#GPIO.output(15, GPIO.HIGH)
GPIO.cleanup()