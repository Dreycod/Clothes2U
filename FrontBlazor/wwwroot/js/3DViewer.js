window.waitForElement = (elementId, maxAttempts = 40) => {
    return new Promise((resolve) => {
        let attempts = 0;

        const checkElement = () => {
            const el = document.getElementById(elementId);

            if (el && el.clientWidth > 0 && el.clientHeight > 0) {
                console.log(`Element ready after ${attempts * 50}ms:`, el.clientWidth, 'x', el.clientHeight);
                resolve(true);
                return;
            }

            attempts++;
            if (attempts >= maxAttempts) {
                console.error(`Element ${elementId} not ready after ${maxAttempts * 50}ms`);
                resolve(false);
                return;
            }

            setTimeout(checkElement, 50);
        };

        checkElement();
    });
};

import * as THREE from 'three';
import { GLTFLoader } from 'jsm/loaders/GLTFLoader.js';
import { OrbitControls } from 'jsm/controls/OrbitControls.js';

window.initAndApplyTexture = (containerId, modelUrl, textureUrl) => {
    console.log('=== 3D Viewer Initialization ===');
    console.log('Container ID:', containerId);
    console.log('Model URL:', modelUrl);
    console.log('Texture URL:', textureUrl);

    const container = document.getElementById(containerId);

    if (!container) {
        console.error(`Container ${containerId} not found`);
        return;
    }

    console.log('Container found:', container);
    console.log('Container dimensions:', container.clientWidth, 'x', container.clientHeight);

    if (!textureUrl || textureUrl === "0" || textureUrl.includes("/0")) {
        container.innerHTML = '<div style="display: flex; align-items: center; justify-content: center; height: 100%; color: #666; font-size: 1.1rem;">Aucune photo ou modèle disponible pour la visualisation 3D</div>';
        return;
    }

    const w = container.clientWidth;
    const h = container.clientHeight;

    if (w === 0 || h === 0) {
        console.error('Container has zero dimensions!', w, h);
        return;
    }

    const renderer = new THREE.WebGLRenderer({ antialias: true });
    renderer.setSize(w, h);
    container.appendChild(renderer.domElement);
    console.log('Renderer created and added to container');

    const camera = new THREE.PerspectiveCamera(45, w / h, 0.1, 1000);
    camera.position.set(0, 0, 7);

    const scene = new THREE.Scene();
    scene.background = new THREE.Color(0xf0f0f0);

    const controls = new OrbitControls(camera, renderer.domElement);
    controls.enableDamping = true;
    controls.dampingFactor = 0.05;
    controls.minDistance = 1.5;
    controls.maxDistance = 5;
    controls.target.set(0, 0, 0);
    controls.update();

    const ambientLight = new THREE.AmbientLight(0xffffff, 0.6);
    scene.add(ambientLight);

    const directionalLight = new THREE.DirectionalLight(0xffffff, 0.8);
    directionalLight.position.set(5, 10, 5);
    scene.add(directionalLight);

    console.log('Loading texture from:', textureUrl);
    const textureLoader = new THREE.TextureLoader();
    const texture = textureLoader.load(
        textureUrl,
        () => console.log('Texture loaded successfully'),
        undefined,
        (err) => console.error('Texture loading error:', err)
    );
    texture.colorSpace = THREE.SRGBColorSpace;
    texture.flipY = false;

    console.log('Loading model from:', modelUrl);
    const loader = new GLTFLoader();
    let model;
    let autoRotate = true;
    let userInteracting = false;
    let interactionTimeout;
    let isZooming = false;

    loader.load(
        modelUrl,
        (gltf) => {
            console.log('Model loaded successfully:', gltf);
            model = gltf.scene;

            model.traverse((child) => {
                if (child.isMesh) {
                    console.log('Applying texture to mesh:', child.name);
                    child.material.map = texture;
                    child.material.needsUpdate = true;
                }
            });

            const box = new THREE.Box3().setFromObject(model);
            const center = box.getCenter(new THREE.Vector3());
            model.position.sub(center);

            scene.add(model);
            console.log('Model added to scene');

            setTimeout(() => {
                animateCameraZoom();
            }, 100);
        },
        (progress) => {
            console.log('Loading progress:', (progress.loaded / progress.total * 100).toFixed(2) + '%');
        },
        (error) => {
            console.error('Model loading error:', error);
        }
    );

    function animateCameraZoom() {
        isZooming = true;
        const startPos = camera.position.clone();
        const endPos = new THREE.Vector3(0, 0, 2.5);
        const duration = 1500;
        const startTime = performance.now();

        function updateZoom() {
            const elapsed = performance.now() - startTime;
            const progress = Math.min(elapsed / duration, 1);
            const eased = 1 - Math.pow(1 - progress, 3);

            camera.position.lerpVectors(startPos, endPos, eased);
            controls.update();

            if (progress < 1) {
                requestAnimationFrame(updateZoom);
            } else {
                isZooming = false;
            }
        }

        updateZoom();
    }

    controls.addEventListener('start', () => {
        userInteracting = true;
        autoRotate = false;
        clearTimeout(interactionTimeout);
    });

    controls.addEventListener('end', () => {
        userInteracting = false;
        interactionTimeout = setTimeout(() => {
            autoRotate = true;
        }, 2000);
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
        controls.update();

        if (model && autoRotate && !userInteracting && !isZooming) {
            model.rotation.y -= 0.005;
        }

        renderer.render(scene, camera);
    }
    animate();
    console.log('Animation loop started');
};

window.clearCanvas = (containerId) => {
    const container = document.getElementById(containerId);
    if (container) {
        console.log('Clearing canvas:', containerId);
        container.innerHTML = '';
    }
};