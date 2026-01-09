import * as THREE from 'https://cdn.jsdelivr.net/npm/three@0.146.0/build/three.module.js';
import { OBJLoader } from 'https://cdn.jsdelivr.net/npm/three@0.146.0/examples/jsm/loaders/OBJLoader.js';
import { OrbitControls } from 'https://cdn.jsdelivr.net/npm/three@0.146.0/examples/jsm/controls/OrbitControls.js';

const scene = new THREE.Scene();
const camera = new THREE.PerspectiveCamera(75, window.innerWidth / window.innerHeight, 0.1, 1000);
//const renderer = new THREE.WebGLRenderer();
const renderer = new THREE.WebGLRenderer({ alpha: true });
scene.background = null;
renderer.setClearColor(0x000000, 0);

renderer.setSize(window.innerWidth, window.innerHeight);
document.body.appendChild(renderer.domElement);
const controls = new OrbitControls(camera, renderer.domElement);
controls.enableDamping = true;
controls.dampingFactor = 0.03;
controls.enableZoom = false

const truckTexture = new THREE.TextureLoader().load('delivery_truck_blue.png');
const material = new THREE.MeshStandardMaterial({
    map: truckTexture,
});

const loader = new OBJLoader();
loader.load(
  'delivery_truck.obj', 
  (object) => {
    object.traverse((child) => {
        if (child.isMesh) {
            child.material = material;
        }
    });
    
    scene.add(object);
  },
  (xhr) => {
    console.log((xhr.loaded / xhr.total * 100) + '% loaded');
  },
  (error) => {
    console.error('Error loading OBJ model:', error);
  }
);

const light = new THREE.AmbientLight(0x808080);
scene.add(light);

camera.position.x = 85.4446931007356;
camera.position.y = 157.0414438001525;
camera.position.z = -578.5007339757639

camera.rotation.x = -2.876515825087479
camera.rotation.y = 0.14158768761112955
camera.rotation.z = 3.103303561417997

function animate() {
    requestAnimationFrame(animate);
    renderer.render(scene, camera);
    controls.update();
}
animate();

function sleep(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}

while (true) {
    await sleep(1000);
    console.log(camera);
}