import * as THREE from 'three';
import { GLTFLoader } from 'jsm/loaders/GLTFLoader.js';

window.initAndApplyTexture = (containerId, modelUrl, textureUrl) => {
    const container = document.getElementById(containerId);

    if (!container) {
        console.error(`Container ${containerId} not found`);
        return;
    }

    if (!textureUrl || textureUrl === "0" || textureUrl.includes("/0")) {
        container.innerHTML = '<div style="display: flex; align-items: center; justify-content: center; height: 100%; color: #666; font-size: 1.1rem;">Aucune photo disponible pour la visualisation 3D</div>';
        return;
    }

    const w = container.clientWidth;
    const h = container.clientHeight;

    const renderer = new THREE.WebGLRenderer({ antialias: true });
    renderer.setSize(w, h);
    container.appendChild(renderer.domElement);

    const camera = new THREE.PerspectiveCamera(45, w / h, 0.1, 1000);
    camera.position.set(0, 0, 3);

    const scene = new THREE.Scene();
    scene.background = new THREE.Color(0xf0f0f0);

    const ambientLight = new THREE.AmbientLight(0xffffff, 0.6);
    scene.add(ambientLight);

    const directionalLight = new THREE.DirectionalLight(0xffffff, 0.8);
    directionalLight.position.set(5, 10, 5);
    scene.add(directionalLight);

    // Load texture
    const textureLoader = new THREE.TextureLoader();
    const texture = textureLoader.load(textureUrl);
    texture.colorSpace = THREE.SRGBColorSpace;
    texture.flipY = false;

    const loader = new GLTFLoader();
    let model;

    loader.load(modelUrl, (gltf) => {
        model = gltf.scene;

        model.traverse((child) => {
            if (child.isMesh) {
                child.material.map = texture;
                child.material.needsUpdate = true;
            }
        });

        const box = new THREE.Box3().setFromObject(model);
        const center = box.getCenter(new THREE.Vector3());
        model.position.sub(center);

        scene.add(model);
    });

    const onResize = () => {
        const w = container.clientWidth;
        const h = container.clientHeight;
        camera.aspect = w / h;
        camera.updateProjectionMatrix();
        renderer.setSize(w, h);
    };
    window.addEventListener('resize', onResize);
    function animate() {
        requestAnimationFrame(animate);
        if (model) {
            model.rotation.y += 0.005;
        }
        renderer.render(scene, camera);
    }
    animate();
};

window.clearCanvas = (containerId) => {
    const container = document.getElementById(containerId);
    if (container) {
        container.innerHTML = '';
    }
};
