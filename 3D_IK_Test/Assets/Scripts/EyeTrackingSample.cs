using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ViveSR
{
    namespace anipal
    {
        namespace Eye
        {

            public class EyeTrackingSample: MonoBehaviour
            {
                //(0)�擾�Ăяo��-----------------------------
                //�Ăяo�����f�[�^�i�[�p�̊֐�
                EyeData eye;
                //-------------------------------------------

                //(1)���E�ʒu--------------------
                //x,y��
                //���̓��E�ʒu�i�[�p�֐�
                Vector2 LeftPupil;
                //���̓��E�ʒu�i�[�p�֐�
                Vector2 RightPupil;
                //------------------------------

                //(2)�܂Ԃ��̊J���------------
                //���̂܂Ԃ��̊J����i�[�p�֐�
                float LeftBlink;
                //�E�̂܂Ԃ��̊J����i�[�p�֐�
                float RightBlink;
                //------------------------------

                //(3)�������--------------------
                //origin�F�N�_�Cdirection�F���C�̕����@x,y,z��
                //���ڂ̎����i�[�ϐ�
                Vector3 CombineGazeRayorigin;
                Vector3 CombineGazeRaydirection;
                //���ڂ̎����i�[�ϐ�
                Vector3 LeftGazeRayorigin;
                Vector3 LeftGazeRaydirection;
                //�E�ڂ̎����i�[�ϐ�
                Vector3 RightGazeRayorigin;
                Vector3 RightGazeRaydirection;
                //------------------------------

                //(4)�œ_���--------------------
                //���ڂ̏œ_�i�[�ϐ�
                //���C�̎n�_�ƕ����i�����B�̓��e�Ɠ����j
                Ray CombineRay;
                /*���C���ǂ��ɏœ_�����킹�����̏��DVector3 point : �����x�N�g���ƕ��̂̏Փˈʒu�Cfloat distance : ���Ă��镨�̂܂ł̋����C
                   Vector3 normal:���Ă��镨�̖̂ʂ̖@���x�N�g���CCollider collider : �Փ˂����I�u�W�F�N�g��Collider�CRigidbody rigidbody�F�Փ˂����I�u�W�F�N�g��Rigidbody�CTransform transform�F�Փ˂����I�u�W�F�N�g��Transform*/
                //�œ_�ʒu�ɃI�u�W�F�N�g���o�����߂�public�ɂ��Ă��܂��D
                public static FocusInfo CombineFocus;
                //���C�̔��a
                float CombineFocusradius;
                //���C�̍ő�̒���
                float CombineFocusmaxDistance;
                //�I�u�W�F�N�g��I��I�ɖ������邽�߂Ɏg�p����郌�C���[ ID
                int CombinefocusableLayer = 0;
                //------------------------------

                public GameObject EyePoint;


                //1�t���[�����Ɏ��s
                void Update()
                {
                    //���܂�------------------------------------
                    //�G���[�m�FViveSR.Error.��WORK�Ȃ琳��ɓ����Ă���D�i�t���[�����[�N�̕��ɓ����ς݂����炢��Ȃ������j
                    if (SRanipal_Eye_API.GetEyeData(ref eye) == ViveSR.Error.WORK)
                    {
                        //�ꉞ�@�킪����ɓ����Ă鎞�̏����������ɂ�����
                    }
                    //-------------------------------------------


                    //(0)�擾�Ăяo��-----------------------------
                    SRanipal_Eye_API.GetEyeData(ref eye);
                    //-------------------------------------------


                    //(1)���E�ʒu---------------------�iHMD����ƌ��m�����C�ڂ��Ԃ��Ă��ʒu�͕Ԃ����CHMD���O���ƂƎ~�܂�D�ڂ��Ԃ��Ă�Ƃ��͂ǂ��̒l�Ԃ��Ă�̂���D�ꉞ�܂Ԃ��ђʂ��Ă���ۂ��H�H�H�j
                    //���̓��E�ʒu���擾
                    if (SRanipal_Eye.GetPupilPosition(EyeIndex.LEFT, out LeftPupil))
                    {
                        //�l���L���Ȃ獶�̓��E�ʒu��\��
                        Debug.Log("Left Pupil" + LeftPupil.x + ", " + LeftPupil.y);
                    }
                    //�E�̓��E�ʒu���擾
                    if (SRanipal_Eye.GetPupilPosition(EyeIndex.RIGHT, out RightPupil))
                    {
                        //�l���L���Ȃ�E�̓��E�ʒu��\��
                        Debug.Log("Right Pupil" + RightPupil.x + ", " + RightPupil.y);
                    }
                    //------------------------------


                    //(2)�܂Ԃ��̊J���------------�iHMD�����ĂȂ��Ă�1���Ԃ��Ă���H�H��j
                    //���̂܂Ԃ��̊J������擾
                    if (SRanipal_Eye.GetEyeOpenness(EyeIndex.LEFT, out LeftBlink, eye))
                    {
                        //�l���L���Ȃ獶�̂܂Ԃ��̊J�����\��
                        Debug.Log("Left Blink" + LeftBlink);
                    }
                    //�E�̂܂Ԃ��̊J������擾
                    if (SRanipal_Eye.GetEyeOpenness(EyeIndex.RIGHT, out RightBlink, eye))
                    {
                        //�l���L���Ȃ�E�̂܂Ԃ��̊J�����\��
                        Debug.Log("Right Blink" + RightBlink);
                    }
                    //------------------------------


                    //(3)�������--------------------�i�ڂ��Ԃ�ƌ��m����Ȃ��j
                    //���ڂ̎�����񂪗L���Ȃ王������\��origin�F�N�_�Cdirection�F���C�̕���
                    if (SRanipal_Eye.GetGazeRay(GazeIndex.COMBINE, out CombineGazeRayorigin, out CombineGazeRaydirection, eye))
                    {
                        Debug.Log("COMBINE GazeRayorigin" + CombineGazeRayorigin.x + ", " + CombineGazeRayorigin.y + ", " + CombineGazeRayorigin.z);
                        Debug.Log("COMBINE GazeRaydirection" + CombineGazeRaydirection.x + ", " + CombineGazeRaydirection.y + ", " + CombineGazeRaydirection.z);
                    }

                    //���ڂ̎�����񂪗L���Ȃ王������\��origin�F�N�_�Cdirection�F���C�̕���
                    if (SRanipal_Eye.GetGazeRay(GazeIndex.LEFT, out LeftGazeRayorigin, out LeftGazeRaydirection, eye))
                    {
                        Debug.Log("Left GazeRayorigin" + LeftGazeRayorigin.x + ", " + LeftGazeRayorigin.y + ", " + LeftGazeRayorigin.z);
                        Debug.Log("Left GazeRaydirection" + LeftGazeRaydirection.x + ", " + LeftGazeRaydirection.y + ", " + LeftGazeRaydirection.z);
                    }


                    //�E�ڂ̎�����񂪗L���Ȃ王������\��origin�F�N�_�Cdirection�F���C�̕���
                    if (SRanipal_Eye.GetGazeRay(GazeIndex.RIGHT, out RightGazeRayorigin, out RightGazeRaydirection, eye))
                    {
                        Debug.Log("Right GazeRayorigin" + RightGazeRayorigin.x + ", " + RightGazeRayorigin.y + ", " + RightGazeRayorigin.z);
                        Debug.Log("Right GazeRaydirection" + RightGazeRaydirection.x + ", " + RightGazeRaydirection.y + ", " + RightGazeRaydirection.z);
                    }
                    //------------------------------

                    //(4)�œ_���--------------------
                    //radius, maxDistance�CCombinefocusableLayer�͏ȗ���
                    if (SRanipal_Eye.Focus(GazeIndex.COMBINE, out CombineRay, out CombineFocus/*, CombineFocusradius, CombineFocusmaxDistance, CombinefocusableLayer*/))
                    {
                        Debug.Log("Combine Focus Point" + CombineFocus.point.x + ", " + CombineFocus.point.y + ", " + CombineFocus.point.z);
                    }
                    //------------------------------

                    //�œ_���I�u�W�F�N�g�̍��W���œ_���ƈ�v
                    EyePoint.transform.position = new Vector3(CombineFocus.point.x, CombineFocus.point.y, CombineFocus.point.z);

                }
            }
        }
    }
}