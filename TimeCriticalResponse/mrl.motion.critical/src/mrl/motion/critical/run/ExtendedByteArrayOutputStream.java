package mrl.motion.critical.run;

import javax.vecmath.*;
import java.io.DataOutputStream;
import java.io.FilterOutputStream;
import java.io.IOException;
import java.io.OutputStream;

public class ExtendedByteArrayOutputStream extends DataOutputStream {

    /**
     * Creates a new data output stream to write data to the specified
     * underlying output stream. The counter {@code written} is
     * set to zero.
     *
     * @param out the underlying output stream, to be saved for later
     *            use.
     * @see FilterOutputStream#out
     */
    public ExtendedByteArrayOutputStream(OutputStream out) {
        super(out);
    }

    public void writeString(String str) throws IOException {
        writeInt(str.length());
        writeChars(str);
    }

    public void writePoint3d(Point3d p3d) throws IOException {
        writeFloat((float)p3d.x);
        writeFloat((float)p3d.y);
        writeFloat((float)p3d.z);
    }

    public void writeVector3d(Vector3d v3d) throws IOException {
        writeFloat((float)v3d.x);
        writeFloat((float)v3d.y);
        writeFloat((float)v3d.z);
    }

    public void writePoint2d(Point2d p2d) throws IOException {
        writeFloat((float)p2d.x);
        writeFloat((float)p2d.y);
    }

    public void writeVector2d(Vector2d v2d) throws IOException {
        writeFloat((float)v2d.x);
        writeFloat((float)v2d.y);
    }

    public void writeMatrix4d(Matrix4d m) throws IOException {
        for (int i = 0; i < 4; i++) {
            for (int j = 0; j < 4; j++) {
                writeFloat((float)m.getElement(i, j));
            }
        }
    }
}
