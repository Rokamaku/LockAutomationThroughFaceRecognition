import threading
import ConfigAll
import sys
import os
from iothub_client import IoTHubClient, IoTHubClientError, IoTHubTransportProvider, IoTHubClientResult, IoTHubError
from iothub_client import IoTHubMessage, IoTHubMessageDispositionResult, IoTHubError, DeviceMethodReturnValue
from iothub_client import IoTHubClientRetryPolicy, GetRetryPolicyReturnValue
import iothub_client_cert as iothub_cert
from queue import Queue
import time
import LockControlThread as lockThread
import json

RECEIVE_CONTEXT = 0
METHOD_CONTEXT = 0

class CloudCommunicatorThread(threading.Thread):
    def __init__(self, queue, lockQueue, args=(), kwargs=None):
        threading.Thread.__init__(self, args=(), kwargs=None)
        self.queue = queue
        self.lockQueue = lockQueue
        self.daemon = True
        self.iot_hub_init()
        
    def iot_hub_init(self):
        self.client = IoTHubClient(ConfigAll.IOT_HUB_CONNECTION_STRING, ConfigAll.CLOUD_COMMUNICATE_PROTOCOL)
        self.client.set_option("TrustedCerts", iothub_cert.CERTIFICATES)
        self.client.set_message_callback(self.receive_message_callback, RECEIVE_CONTEXT)
        self.client.set_device_method_callback(self.device_method_callback, METHOD_CONTEXT)
        
    def run(self):
        while True:
            signal = self.queue.get()
            if signal is None:
                print("Cloud communicator terminated")
                break
            
    def device_method_callback(self, method_name, payload, user_context):
        print ( "\nMethod callback called with:\nmethodName = %s\npayload = %s\ncontext = %s" % (method_name, payload, user_context) )
        key_value_pair = json.loads(payload)
        self.callLockControlThread(key_value_pair)
        device_method_return_value = DeviceMethodReturnValue()
        device_method_return_value.response = "{ \"Result\": \"Invoke successfully\" }"
        device_method_return_value.status = 200
        return device_method_return_value
            
    def receive_message_callback(self, message, counter):
        message_buffer = message.get_bytearray()
        size = len(message_buffer)
        print ( "Received Message [%d]:" % counter )
        print ( "    Data: <<<%s>>> & Size=%d" % (message_buffer[:size].decode('utf-8'), size) )
        map_properties = message.properties()
        key_value_pair = map_properties.get_internals()
        print ( "    Properties: %s" % key_value_pair )
        self.callLockControlThread(key_value_pair)
        return IoTHubMessageDispositionResult.ACCEPTED
    
    def callLockControlThread(self, key_value_pair):
        if key_value_pair[ConfigAll.ISON] != None and \
            key_value_pair[ConfigAll.TIME] != None:
            print("thread != None " + key_value_pair[ConfigAll.ISON])
            self.lockQueue.put(key_value_pair)

