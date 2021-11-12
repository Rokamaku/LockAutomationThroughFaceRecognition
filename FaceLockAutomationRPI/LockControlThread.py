import RPi.GPIO as GPIO
import time
import threading
import ConfigAll
import traceback

class LockControlThread(threading.Thread):
    def __init__(self, queue):
        threading.Thread.__init__(self)
        self.daemon = True
        self.queue = queue
        self.isOpen = False
        
        self.setUpPin()

        
    def setUpPin(self):
        GPIO.setmode(GPIO.BCM)
        GPIO.setup(ConfigAll.ServoPin, GPIO.OUT)
        GPIO.setup(ConfigAll.LockPin, GPIO.OUT)
        GPIO.setup(ConfigAll.PWMServoPin, GPIO.OUT)
        
        GPIO.output(ConfigAll.LockPin, GPIO.HIGH)
        GPIO.output(ConfigAll.ServoPin, GPIO.HIGH)
        self.servo = GPIO.PWM(ConfigAll.PWMServoPin, 50)
        
    def run(self):
        try:
            while True:
                if self.isOpen == True and self.queue.empty():
#                    print("self.isOpen == True and self.queue.empty()")
                    self.closeDoor()
                    continue
                elif self.isOpen == False and self.queue.empty():
#                    print("self.isOpen == False and self.queue.empty()")
                    continue
                else:
                    signal = self.queue.get()
                    if signal is None and self.isOpen == True:
#                        print("signal is None and self.isOpen == True")
                        print("Lock terminated")
                        self.closeDoor()
                        break
                    elif signal is None and self.isOpen == False:
#                        print("signal is None and self.isOpen == False")
                        print("Lock terminated")
                        break
                    elif self.isOpen == False and signal[ConfigAll.ISON] == "True":
#                        print("isOpen == False and signal.ISON == True")
                        print("Open door time " + str(time.clock()))
                        self.openDoor()
                        print("time: " + str(float(signal[ConfigAll.TIME])))
                        time.sleep(5)
#                        time.sleep(float(signal[ConfigAll.TIME]))
                        continue
                    elif self.isOpen == True and signal[ConfigAll.ISON] == "True":
#                        print("isOpen == True and signal.ISON == True")
                        print("Open door time " + str(time.clock()))
                        time.sleep(5)
#                        time.sleep(float(signal[ConfigAll.TIME]))
                        continue
                    elif self.isOpen == True and signal[ConfigAll.ISON] == "False":
#                        print("isOpen == True and signal.ISON == False")
                        self.closeDoor()                    
        except KeyboardInterrupt:
            print("Control Relay stopped")
        except Exception as ex:
            print("Generic Error on running Lock Controller")
            print(ex)
            traceback.print_exc()
        finally:
            self.cleanUp()
        
    def openDoor(self):
        try:
            GPIO.output(ConfigAll.ServoPin, GPIO.LOW)
            GPIO.output(ConfigAll.LockPin, GPIO.LOW)            
            self.servo.start(2)
            time.sleep(1)
#            self.servo.ChangeDutyCycle(8)
#            time.sleep(1)
            GPIO.output(ConfigAll.LockPin, GPIO.HIGH)
#            self.servo.stop()
            GPIO.output(ConfigAll.ServoPin, GPIO.HIGH)
            print("Open door")
            self.isOpen = True
        except KeyboardInterrupt:
            print("Control Relay stopped")
        except Exception as ex:
            print("Generic Error on turning on Relay")
            print(ex)
            traceback.print_exc()
             
        
    def closeDoor(self):
        try:
            GPIO.output(ConfigAll.ServoPin, GPIO.LOW)
#            GPIO.output(ConfigAll.LockPin, GPIO.LOW)
            self.servo.ChangeDutyCycle(7.5)
            time.sleep(2)
            GPIO.output(ConfigAll.ServoPin, GPIO.HIGH)
#            GPIO.output(ConfigAll.LockPin, GPIO.HIGH)
            print("Close door")
            self.isOpen = False
        except KeyboardInterrupt:
            print("Control Relay stopped")
        except Exception as ex:
            print("Generic Error on turning off Relay")
            print(ex)
            traceback.print_exc()
            
            
    def cleanUp(self):
        self.servo.stop()
        GPIO.cleanup()
        