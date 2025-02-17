// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using PhantoUtils.VR;
using UnityEngine;

public class SoftParent : MonoBehaviour
{
    public enum ParentTargetType
    {
        Camera,
        LeftHand,
        LeftController,
        RightHand,
        RightController,
        Transform
    }

    [SerializeField] private ParentTargetType parentTarget;
    [SerializeField] private Transform parentTransform;
    [SerializeField] private Vector3 offset;

    private Transform _targetTransform;

    public ParentTargetType ParentTarget
    {
        get => parentTarget;
        set
        {
            parentTarget = value;
            FindTargetTransform();
        }
    }

    public Transform TargetTransform
    {
        get => _targetTransform;
        set
        {
            parentTransform = value;
            _targetTransform = value;
            parentTarget = ParentTargetType.Transform;
        }
    }



    void Awake()
    {
        _targetTransform = FindTargetTransform();
    }


    void Update ()
    {
        AttachToChosenParent();
    }


    void AttachToChosenParent()
    {
        if (_targetTransform == null)
        {
            // We could check this in Start, but checking on first update allows for late transform attaching
            _targetTransform = FindTargetTransform();
            if (_targetTransform == null) enabled = false;
        }

        transform.position = _targetTransform.TransformPoint(offset);
        transform.rotation = _targetTransform.rotation;

        FixTransform();
    }


    private void FixTransform()
    {
        // BUGFIX: For some reason, the Polterblast attaches to the hand rotated.
		// This function offsets the transform to make the Polterblast gripped as a gun when hand tracking is active.
        if (parentTarget == ParentTargetType.RightHand)
        {
            transform.Translate(-0.09f, -0.02f, 0);
            transform.Rotate(0, -90, 90);
        }
        else if (parentTarget == ParentTargetType.LeftHand)
        {
            transform.Translate(0.09f, 0.02f, 0);
            transform.Rotate(180, -90, -90);
        }
    }
	

    Transform FindTargetTransform()
    {
        switch (parentTarget)
        {
            case ParentTargetType.Camera:
                return CameraRig.Instance != null ? CameraRig.Instance.CenterEyeAnchor : null;
            case ParentTargetType.LeftHand:
                return CameraRig.Instance != null ? CameraRig.Instance.LeftHandAnchor : null;
            case ParentTargetType.LeftController:
                return CameraRig.Instance != null ? CameraRig.Instance.LeftControllerAnchor : null;
            case ParentTargetType.RightHand:
                return CameraRig.Instance != null ? CameraRig.Instance.RightHandAnchor : null;
            case ParentTargetType.RightController:
                return CameraRig.Instance != null ? CameraRig.Instance.RightControllerAnchor : null;
            case ParentTargetType.Transform:
                return parentTransform;
            default:
                throw new ArgumentOutOfRangeException(nameof(parentTarget), parentTarget, null);
        }
    }
}
