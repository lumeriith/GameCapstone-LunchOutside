import tensorflow as tf
tf = tf.compat.v1
tf.disable_eager_execution()
from rl.GMM import GMM
import numpy as np

np.set_printoptions(formatter={'float_kind':lambda x:"%.6f"%x})

model = GMM(folder, num_state, num_action)
model.initialize(useCPU)
# print("Before Load")
# sys.stdout.flush()
            