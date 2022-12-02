package mrl.motion.critical.run;

import javax.vecmath.Vector2d;
import java.io.IOException;

public class ModelMessageHandler {
    public ModelBackendServer parent;

    public ModelMessageHandler(ModelBackendServer server) {
        parent = server;
    }

    private interface MessageHandlerInterface {
        void handle(int payloadLength) throws IOException;
    }

    private final MessageHandlerInterface[] messageHandlers = {
            this::handleDoAction,
            this::handleSetDirection,
            this::handleSetTimescale,
            this::handleSetTotalAgility,
    };

    public void handleMessage(int opCode, int length) throws IOException {
        if (opCode < 0 || opCode >= messageHandlers.length){
            parent.socketReader.skipBytes(length);
            System.out.println("Got unknown opcode " + opCode + " with payload length of " + length);
            return;
        }
        System.out.println("OP" + opCode + ", Payload: " + length);
        messageHandlers[opCode].handle(length);
    }

    private void handleDoAction(int length) throws IOException {
        int action = parent.socketReader.readInt();
        parent.model.doAction(action);
    }

    private void handleSetDirection(int length) throws IOException {
        parent.model.setDirection(new Vector2d(parent.socketReader.readFloat(), parent.socketReader.readFloat()));
    }

    private void handleSetTimescale(int length) throws IOException {
        parent.timescale = parent.socketReader.readFloat();
    }
    
    private void handleSetTotalAgility(int length) throws IOException {
    	parent.model.setTotalAgility(parent.socketReader.readDouble());
    }
}
