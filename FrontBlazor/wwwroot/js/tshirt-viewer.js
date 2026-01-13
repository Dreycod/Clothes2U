import * as THREE from 'https://cdn.jsdelivr.net/npm/three@0.164.1/build/three.module.js';
import { GLTFLoader } from 'https://cdn.jsdelivr.net/npm/three@0.164.1/examples/jsm/loaders/GLTFLoader.js';

class TShirtViewer {
    constructor(containerId) {
        this.container = document.getElementById(containerId);
        if (!this.container) throw new Error(`Container with id "${containerId}" not found`);

        this.scene = null;
        this.camera = null;
        this.renderer = null;
        this.model = null;
        this.animationId = null;

        this.rotationSpeed = 0.005;

        this.init();
    }

    init() {
        this.scene = new THREE.Scene();
        this.scene.background = new THREE.Color(0xf8f9fa);

        const width = this.container.clientWidth;
        const height = this.container.clientHeight;
        this.camera = new THREE.PerspectiveCamera(45, width / height, 0.1, 1000);
        this.camera.position.set(0, 0, 3);

        this.renderer = new THREE.WebGLRenderer({ antialias: true });
        this.renderer.setSize(width, height);
        this.renderer.setPixelRatio(Math.min(window.devicePixelRatio, 2));
        this.container.appendChild(this.renderer.domElement);

        const ambientLight = new THREE.AmbientLight(0xffffff, 0.6);
        this.scene.add(ambientLight);

        const directionalLight = new THREE.DirectionalLight(0xffffff, 0.8);
        directionalLight.position.set(5, 10, 5);
        this.scene.add(directionalLight);

        this._onResize = () => this.onWindowResize();
        window.addEventListener('resize', this._onResize);

        this.animate();
    }

    loadModel(modelUrl) {
        const loader = new GLTFLoader();

        return new Promise((resolve, reject) => {
            loader.load(
                modelUrl,
                (gltf) => {
                    this.model = gltf.scene;

                    const box = new THREE.Box3().setFromObject(this.model);
                    const center = box.getCenter(new THREE.Vector3());
                    this.model.position.sub(center);

                    this.scene.add(this.model);
                    resolve(this.model);
                },
                undefined,
                (error) => {
                    console.error('Error loading model:', error);
                    reject(error);
                }
            );
        });
    }

    onWindowResize() {
        const width = this.container.clientWidth;
        const height = this.container.clientHeight;

        this.camera.aspect = width / height;
        this.camera.updateProjectionMatrix();
        this.renderer.setSize(width, height);
    }

    animate() {
        this.animationId = requestAnimationFrame(() => this.animate());

        if (this.model) {
            this.model.rotation.y += this.rotationSpeed;
        }

        this.renderer.render(this.scene, this.camera);
    }

    dispose() {
        if (this.animationId) cancelAnimationFrame(this.animationId);

        if (this.renderer) {
            this.renderer.dispose();
            this.container.removeChild(this.renderer.domElement);
        }

        if (this.model) this.scene.remove(this.model);

        window.removeEventListener('resize', this._onResize);
    }
}

window.TShirtViewerInstances = window.TShirtViewerInstances || {};

window.initTShirtViewer = async (containerId, modelUrl) => {
    const viewer = new TShirtViewer(containerId);
    window.TShirtViewerInstances[containerId] = viewer;

    try {
        await viewer.loadModel(modelUrl);
        return true;
    } catch {
        return false;
    }
};

window.disposeTShirtViewer = (containerId) => {
    const viewer = window.TShirtViewerInstances[containerId];
    if (viewer) {
        viewer.dispose();
        delete window.TShirtViewerInstances[containerId];
    }
};